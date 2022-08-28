using UnityEngine;
using Unity.Collections;

namespace Sketch4 {

static class Util
{
    public static NativeArray<T> AllocateBuffer<T>(uint length)
      where T : unmanaged
      => new NativeArray<T>((int)length, Allocator.Persistent,
                            NativeArrayOptions.UninitializedMemory);

    public static void DestroyObject(Object o)
    {
        if (o == null) return;
        if (Application.isPlaying)
            Object.Destroy(o);
        else
            Object.DestroyImmediate(o);
    }
}

} // namespace Sketch4
