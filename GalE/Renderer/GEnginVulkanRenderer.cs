//TODO: add Vulkan ValidationLayers and Debug Magger

using GalE.Core;
using GalE.Util;
using GalE.Renderer;

using static SDL2.SDL;
using System.Diagnostics;

using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;

namespace GalE;

public unsafe class GEnginVulkanRenderer : GEnginRenderer
{
    public Instance Instance;
    public DebugUtilsMessengerEXT DebugMessenger;
    public PhysicalDevice PhysicalDevice;
    public Device Device;
    public SurfaceKHR Surface;
    public Queue GraphicsQueue;

    private bool boolResult = true;
    private Result result = Result.Success;
    private GEnginWindow? enginWindow = null;

    private Vk vk;
    private KhrSurface vkSurface;
    private KhrSwapchain vkSwapChain;
    private ExtDebugUtils? debugUtils;
    private DebugUtilsMessengerEXT debugMessenger;

#if DEBUG
    private const bool EnableValidationLayers = true;
    public string[] ValidationLayers =
    {
        "VK_LAYER_KHRONOS_validation"
    };
#else
    private const bool EnableValidationLayers = false;
#endif

    struct QueueFamilyIndices
    {
        public uint? GraphicsFamily { get; set; }
        public bool IsComplete()
        {
            return GraphicsFamily.HasValue;
        }
    }

    public override void Init(GEnginWindow window)
    {
        Inited.Check(true);
        enginWindow = window;
        CreateVulkanInstance();
        CreateSurface();
        // SetupDebugMessenger();
        PickPyhsicalDevice();
        CreateLogicalDevice();
        Inited.Init();
    }

    public override void Dispose()
    {
        if (EnableValidationLayers)
        {
            debugUtils!.DestroyDebugUtilsMessenger(Instance, debugMessenger, null);
        }

        Inited.Check();
        vk.DestroyDevice(Device, null);
        vkSurface.DestroySurface(Instance, Surface, null);
        vk.DestroyInstance(Instance, null);
        vk.Dispose();
        GC.SuppressFinalize(this);
    }

    private void CreateVulkanInstance()
    {
        vk = Vk.GetApi();

        // if (EnableValidationLayers && !CheckValidationLayerSupport())
        // {
        //     throw new Exception("validation layers requested, but not available!");
        // }

        ApplicationInfo appInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*) Marshal.StringToHGlobalAnsi(enginWindow.GEngin.GameName),
            ApplicationVersion = (VkVersion)enginWindow.GEngin.GameVersion.CoreVersion,
            EngineVersion = (VkVersion)GalE.GalEVersion.CoreVersion,
            PEngineName = (byte*) Marshal.StringToHGlobalAnsi("Gal Engin"),
            ApiVersion = Vk.Version11
        };
        var extensions = GetRequiredExtensions();

        InstanceCreateInfo createInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &appInfo,
            EnabledExtensionCount = (uint)extensions.Length,
            PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(extensions),
            EnabledLayerCount = 0,
            PNext = null
        };

        // if (EnableValidationLayers)
        // {
        //     createInfo.EnabledLayerCount = (uint)ValidationLayers.Length;
        //     createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(ValidationLayers);

        //     DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new();
        //     PopulateDebugMessengerCreateInfo(ref debugCreateInfo);
        //     createInfo.PNext = &debugCreateInfo;
        // }
        // else
        // {
            createInfo.EnabledLayerCount = 0;
            createInfo.PNext = null;
        // }

        result = vk.CreateInstance(&createInfo, null, out Instance);
        Debug.Assert(result == Result.Success, "Create Instance Error");

        vk.CurrentInstance = Instance;

        boolResult = vk.TryGetInstanceExtension(Instance, out vkSurface);
        Debug.Assert(boolResult == true, "KHR_surface extension not found.");

        Marshal.FreeHGlobal((nint) appInfo.PApplicationName);
        Marshal.FreeHGlobal((nint) appInfo.PEngineName);

        // if (EnableValidationLayers)
        // {
        //     SilkMarshal.Free((nint) createInfo.PpEnabledLayerNames);
        // }
    }

    private string[] GetRequiredExtensions()
    {
        SDL_Vulkan_GetInstanceExtensions(enginWindow.SDLWindow, out uint SDLExtensionCount, null);
        IntPtr SDLExtensions = SilkMarshal.AllocateString((int)SDLExtensionCount);
        SDL_Vulkan_GetInstanceExtensions(enginWindow.SDLWindow, out SDLExtensionCount, SDLExtensions);
        string[] extensions = SilkMarshal.PtrToStringArray(SDLExtensions, (int)SDLExtensionCount);

        // if (EnableValidationLayers)
        // {
        //     return extensions.Append(ExtDebugUtils.ExtensionName).ToArray();
        // }

        return extensions;
    }

    private void CreateSurface()
    {
        if (!vk!.TryGetInstanceExtension<KhrSurface>(Instance, out vkSurface))
        {
            throw new NotSupportedException("KHR_surface extension not found.");
        }

        SDL_Vulkan_CreateSurface(enginWindow.SDLWindow, Instance.Handle, out Surface.Handle);
    }

    private void PickPyhsicalDevice()
    {
        uint deviceCount = 0;
        vk.EnumeratePhysicalDevices(Instance, ref deviceCount, null);
        Debug.Assert(deviceCount != 0);

        if (deviceCount == 0)
        {
            throw new Exception("failed to find GPUs with Vulkan support!");
        }

        var devices = new PhysicalDevice[deviceCount];
        fixed (PhysicalDevice* devicesPtr = devices)
        {
            vk!.EnumeratePhysicalDevices(Instance, ref deviceCount, devicesPtr);
        }

        foreach (var device in devices)
        {
            if (FindQueueFamilies(device).IsComplete())
            {
                PhysicalDevice = device;
                break;
            }
        }

        if (PhysicalDevice.Handle == 0)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    private QueueFamilyIndices FindQueueFamilies(PhysicalDevice device)
    {
        var indices = new QueueFamilyIndices();

        uint queueFamilityCount = 0;
        vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, null);

        var queueFamilies = new QueueFamilyProperties[queueFamilityCount];
        fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies)
        {
            vk!.GetPhysicalDeviceQueueFamilyProperties(device, ref queueFamilityCount, queueFamiliesPtr);
        }


        uint i = 0;
        foreach (var queueFamily in queueFamilies)
        {
            if (queueFamily.QueueFlags.HasFlag(QueueFlags.GraphicsBit))
            {
                indices.GraphicsFamily = i;
            }

            if (indices.IsComplete())
            {
                break;
            }

            i++;
        }

        return indices;
    }


    private void CreateLogicalDevice()
    {
        // 设置渲染队列的优先级
        float queuePriority = 1.0f;

        // 创建设备队列创建信息
        DeviceQueueCreateInfo queueCreateInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            QueueFamilyIndex = 0,
            QueueCount = 1,
            PQueuePriorities = &queuePriority
        };

        // 创建物理设备特征
        PhysicalDeviceFeatures deviceFeatures = new();

        // 创建设备创建信息
        DeviceCreateInfo createInfo = new()
        {
            SType = StructureType.DeviceCreateInfo,
            PQueueCreateInfos = &queueCreateInfo,
            QueueCreateInfoCount = 1,
            PEnabledFeatures = &deviceFeatures,
            EnabledLayerCount = 0
        };

        // 创建设备
        result = vk.CreateDevice(PhysicalDevice, &createInfo, null, out Device);
        Debug.Assert(result == Result.Success);

        // 获取设备的渲染队列
        vk.GetDeviceQueue(Device, 0, 0, out GraphicsQueue);
    }
}
