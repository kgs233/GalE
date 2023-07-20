namespace GalE.Util;

public record Version(CoreVersion CoreVersion, string? Prelease = null, string? Build = null)
{
    public string StrVersion = CoreVersion.StrVersion + Prelease ?? "" +Build ?? "";
}

public record CoreVersion(uint Major, uint Minor, uint Revision)
{
    public string StrVersion = Major.ToString() + "." + Minor.ToString() + "." + Revision.ToString();

    public static implicit operator VkVersion(CoreVersion CoreVersion) => new(0, CoreVersion.Major, CoreVersion.Minor, CoreVersion.Revision);
}

/*

BSD 3-Clause License

Copyright (c) 2018-2023, exomia
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/
public readonly struct VkVersion
{
    public static readonly VkVersion VulkanApiVersion10 = new VkVersion(0, 1, 0, 0);

    public static readonly VkVersion VulkanApiVersion11 = new VkVersion(0, 1, 1, 0);

    public static readonly VkVersion VulkanApiVersion12 = new VkVersion(0, 1, 2, 0);

    public static readonly VkVersion VulkanApiVersion13 = new VkVersion(0, 1, 3, 0);

    private readonly uint _version;

    public uint Variant
    {
        get { return _version >> 29; }
    }

    public uint Major
    {
        get { return (_version >> 22) & 0x7FU; }
    }

    public uint Minor
    {
        get { return (_version >> 12) & 0x3FF; }
    }

    public uint Patch
    {
        get { return _version & 0xFFF; }
    }

    public VkVersion(uint variant, uint major, uint minor, uint patch)
        : this((variant << 29) | ((major & 0x7FU) << 22) | ((minor & 0x3FFU) << 12) | (patch & 0xFFFU)) { }

    internal VkVersion(uint version)
    {
        _version = version;
    }

    public override int GetHashCode()
    {
        return (int)_version;
    }

    public bool Equals(VkVersion other)
    {
        return _version == other._version;
    }

    public override bool Equals(object? obj)
    {
        return obj is VkVersion other && Equals(other);
    }

    public override string ToString()
    {
        return $"{Major.ToString()}.{Minor.ToString()}.{Patch.ToString()}";
    }

    public static implicit operator uint(VkVersion version)
    {
        return version._version;
    }
}