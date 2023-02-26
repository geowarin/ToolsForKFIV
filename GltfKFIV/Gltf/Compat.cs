using System.Numerics;
using FormatKFIV.Utility;

namespace GltfKFIV.Gltf;

static class Compat
{
    public static Vector3 ToVector3(this Vector3f v) => new(v.X, v.Y, v.Z);
}