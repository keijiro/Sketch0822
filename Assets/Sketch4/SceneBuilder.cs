using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System;

namespace Sketch4 {

[Serializable]
struct SceneConfig
{
    public int PoleCount;
    public float BaseRange;
    public float NodeStride;
    public float EmissionRate;
    public Color EmissionColor1;
    public Color EmissionColor2;
    public float EmissionIntensity;
    public uint Seed;

    public static SceneConfig Default()
      => new SceneConfig()
        { PoleCount = 80,
          BaseRange = 1.25f,
          NodeStride = 0.1f,
          EmissionRate = 0.4f,
          EmissionColor1 = Color.red,
          EmissionColor2 = Color.blue,
          EmissionIntensity = 600,
          Seed = 10 };
}

static class SceneBuilder
{
    public static int Build
      (SceneConfig cfg,
       (GeometryCache board, GeometryCache pole) shapes,
       Span<Modeler> outBuffer)
    {
        var outCount = 0;

        // PRNG
        var (hash, seed) = (new XXHash(cfg.Seed), 0u);

        // Pole population
        for (var i = 0; i < cfg.PoleCount; i++)
        {
            // Position / angle
            var pos = math.float3(hash.InCircle(seed++) * cfg.BaseRange, 0);
            var angle = hash.Bool(seed++) ? 0 : math.PI / 4;

            // Emitter
            var emitter = hash.Float(seed++) < cfg.EmissionRate;
            var ecolor = hash.Bool(seed++) ? cfg.EmissionColor1 : cfg.EmissionColor2;
            ecolor *= cfg.EmissionIntensity;

            // Probability decay coefficient
            var decay = hash.Float(0.1f, 0.96f, seed++);

            for (var prob = 1.0f; prob > 0.2f;)
            {
                for (var k = 0; k < 4; k++)
                {
                    // Rotation
                    var rot = float2x2.Rotate(angle);

                    // Board
                    var d1 = math.float2(cfg.NodeStride * 0.5f, 0);
                    var p1 = pos + math.float3(math.mul(rot, d1), 0);

                    // Pole
                    var d2 = (float2)(cfg.NodeStride * 0.5f);
                    var p2 = pos + math.float3(math.mul(rot, d2), 0);

                    // Modeler addition
                    if (hash.Float(seed++) < prob)
                        outBuffer[outCount++] = 
                          new Modeler(position: p1,
                                      rotation: angle,
                                      color: Color.black,
                                      shape: shapes.board);

                    outBuffer[outCount++] =
                      new Modeler(position: p2,
                                  rotation: angle,
                                  color: Color.black,
                                  shape: shapes.pole);

                    if (emitter)
                        outBuffer[outCount++] =
                          new Modeler(position: pos,
                                      rotation: 0,
                                      color: ecolor,
                                      shape: shapes.pole);

                    // Rotation advance
                    angle += math.PI / 2;
                }

                // Stride
                pos.z += cfg.NodeStride;

                // Probability decay
                prob *= hash.Float(decay, 1.0f, seed++);
            }

            // Emitter extension
            if (emitter)
            {
                var ext = hash.Int(10, seed++);
                for (var j = 0; j < ext; j++)
                {
                    outBuffer[outCount++] =
                      new Modeler(position: pos,
                                  rotation: 0,
                                  color: ecolor,
                                  shape: shapes.pole);
                    pos.z += cfg.NodeStride;
                }
            }
        }

        return outCount;
    }
}

} // namespace Sketch4
