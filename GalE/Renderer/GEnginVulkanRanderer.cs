using GalE.Core;
using GalE.Util;
using GalE.Renderer;

using static SDL2.SDL;
using System.Diagnostics;

using Vulkan;
using static Vulkan.VulkanNative;
using System.Runtime.InteropServices;

namespace GalE;

public unsafe class GEnginVulkanRenderer : GEnginRenderer
{
    internal VkInstance Instance;
    internal VkPhysicalDevice PhysicalDevice;
    internal VkDevice Device;
    internal VkSurfaceKHR Surface;
    internal VkQueue GraphicsQueue;

    private VkResult result = VkResult.Success;
    private GEnginWindow? enginWindow = null;

    public override void Init(GEnginWindow window)
    {
        Inited.Check(true);
        enginWindow = window;
        CreateVulkanInstance();
        CreateSurface();
        PickPyhsicalDevice();
        CreateLogicalDevice();
        Inited.Init();
    }

    public override void Dispose()
    {
        Inited.Check();
        vkDestroyDevice(Device, IntPtr.Zero);
        vkDestroySurfaceKHR(Instance, Surface, IntPtr.Zero);
        vkDestroyInstance(Instance, IntPtr.Zero);
        GC.SuppressFinalize(this);
    }

    private void CreateVulkanInstance()
    {
        VkApplicationInfo appInfo = new()
        {
            sType = VkStructureType.ApplicationInfo,
            pApplicationName = PtrUtils.StrToBytePointer(enginWindow.GEngin.GameName),
            applicationVersion = (VkVersion)enginWindow.GEngin.GameVersion.CoreVersion,
            engineVersion = (VkVersion)GalE.GalEVersion.CoreVersion,
            pEngineName = PtrUtils.StrToBytePointer("Gal Engin"),
            apiVersion = VkVersion.VulkanApiVersion12
        };
        SDL_Vulkan_GetInstanceExtensions(enginWindow.SDLWindow, out uint SDLExtensionCount, null);
        IntPtr[] SDLExtensions = new IntPtr[SDLExtensionCount];
        SDL_Vulkan_GetInstanceExtensions(enginWindow.SDLWindow, out SDLExtensionCount, SDLExtensions);

        byte** vSDLExtensions = stackalloc byte*[(int)SDLExtensionCount];
        for(int i = 0; i < SDLExtensionCount; i++)
        {
            vSDLExtensions[i] = (byte*)SDLExtensions[i];
        }

        VkInstanceCreateInfo createInfo = new()
        {
            sType = VkStructureType.InstanceCreateInfo,
            pApplicationInfo = &appInfo,
            enabledExtensionCount = SDLExtensionCount,
            ppEnabledExtensionNames = vSDLExtensions,
            enabledLayerCount = 0
        };

        result = vkCreateInstance(&createInfo, null, out Instance);
        Debug.Assert(result == VkResult.Success);
    }

    private void CreateSurface()
    {
        SDL_Vulkan_CreateSurface(enginWindow.SDLWindow, Instance.Handle, out ulong surface);
        Surface = new(surface);
    }

    private void PickPyhsicalDevice()
    {
        uint deviceCount = 0;
        vkEnumeratePhysicalDevices(Instance, ref deviceCount, null);
        Debug.Assert(deviceCount != 0);
        fixed(VkPhysicalDevice* devices = new VkPhysicalDevice[(int)deviceCount])
        {
            vkEnumeratePhysicalDevices(Instance, ref deviceCount, devices);
            PhysicalDevice = devices[0]; 
        }
    }

    private void CreateLogicalDevice()
    {
        float queuePriority = 1.0f;

        VkDeviceQueueCreateInfo queueCreateInfo = new()
        {
            sType = VkStructureType.DeviceCreateInfo,
            queueFamilyIndex = 0,
            queueCount = 1,
            pQueuePriorities = &queuePriority
        };

        VkPhysicalDeviceFeatures deviceFeatures = new();

        VkDeviceCreateInfo createInfo = new()
        {
            sType = VkStructureType.DeviceCreateInfo,
            pQueueCreateInfos = &queueCreateInfo,
            queueCreateInfoCount = 1,
            pEnabledFeatures = &deviceFeatures,
            enabledLayerCount = 0
        };

        result = vkCreateDevice(PhysicalDevice, &createInfo, IntPtr.Zero, out Device);
        Debug.Assert(result == VkResult.Success);

        vkGetDeviceQueue(Device, 0, 0, out GraphicsQueue);
    }
}
