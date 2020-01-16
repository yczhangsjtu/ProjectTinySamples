using System.Collections.Generic;
using Tetric3D;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny.Input;
using Unity.Transforms;
using KeyCode = Unity.Tiny.Input.KeyCode;

namespace Tiny3D
{
    public class Input : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Level>();
        }
        
        protected override void OnUpdate()
        {
            var level = GetSingleton<Level>();
            var inputSystem = World.GetExistingSystem<InputSystem>();
            if (!level.started)
            {
                if (inputSystem.GetKeyDown(KeyCode.Space))
                {
                    level.started = true;
                    SetSingleton(level);
                    GenerateNextShape.random = new Random((uint) (Time.ElapsedTime * 1000));
                }

                return;
            }

            level.speed = inputSystem.GetKey(KeyCode.Space) ? level.fastSpeed : 1;
            SetSingleton(level);
            
            float4x4 matrix = float4x4.identity;
            bool updateCenterPos = false;
            if (inputSystem.GetKeyDown(KeyCode.LeftArrow))
            {
                matrix = float4x4.Translate(new float3(0, -1, 0));
            }
            else if(inputSystem.GetKeyDown(KeyCode.RightArrow))
            {
                matrix = float4x4.Translate(new float3(0, 1, 0));
            }
            else if(inputSystem.GetKeyDown(KeyCode.UpArrow))
            {
                matrix = float4x4.Translate(new float3(-1, 0, 0));
            }
            else if(inputSystem.GetKeyDown(KeyCode.DownArrow))
            {
                matrix = float4x4.Translate(new float3(1, 0, 0));
            }
            else if(inputSystem.GetKeyDown(KeyCode.A))
            {
                matrix = float4x4.RotateX(math.PI / 2);
                updateCenterPos = true;
            }
            else if(inputSystem.GetKeyDown(KeyCode.D))
            {
                matrix = float4x4.RotateX(-math.PI / 2);
                updateCenterPos = true;
            }
            else if(inputSystem.GetKeyDown(KeyCode.Q))
            {
                matrix = float4x4.RotateZ(math.PI / 2);
                updateCenterPos = true;
            }
            else if(inputSystem.GetKeyDown(KeyCode.E))
            {
                matrix = float4x4.RotateZ(-math.PI / 2);
                updateCenterPos = true;
            }
            else if(inputSystem.GetKeyDown(KeyCode.W))
            {
                matrix = float4x4.RotateY(-math.PI / 2);
                updateCenterPos = true;
            }
            else if(inputSystem.GetKeyDown(KeyCode.S))
            {
                matrix = float4x4.RotateY(math.PI / 2);
                updateCenterPos = true;
            }
            else if(inputSystem.GetKeyDown(KeyCode.R))
            {
                level.reset = true;
                SetSingleton(level);
            }
            else if(inputSystem.GetKeyDown(KeyCode.P))
            {
                level.started = false;
                SetSingleton(level);
            }
            else
            {
                matrix = float4x4.identity;
            }

            if (!matrix.Equals(float4x4.identity))
            {
                List<Cube> newCubes = new List<Cube>();
                bool conflict = false;
                Entities.ForEach((Entity entity, ref Dropping dropping, ref Cube cube) =>
                {
                    if (conflict)
                    {
                        return;
                    }
                    var newCube = MoveCube(dropping, cube, matrix);
                    if (newCube.x < 0 || newCube.x > 3 || newCube.y < 0 || newCube.y > 3 || newCube.h < 0)
                    {
                        conflict = true;
                        return;
                    }
                    newCubes.Add(newCube);
                });

                if (newCubes.Count == 0 || conflict)
                {
                    return;
                }
                
                Entities.WithNone<Dropping>().ForEach((ref Cube cube) =>
                {
                    if (conflict)
                    {
                        return;
                    }
                    foreach (var newCube in newCubes)
                    {
                        if (newCube.x == cube.x &&
                            newCube.y == cube.y &&
                            newCube.h == cube.h)
                        {
                            conflict = true;
                            return;
                        }
                    }
                });

                if (conflict)
                {
                    return;
                }

                Entities.ForEach((Entity entity, ref Dropping dropping, ref Cube cube) =>
                {
                    var newCube = MoveCube(dropping, cube, matrix);
                    if (updateCenterPos)
                    {
                        dropping.cx += newCube.x - cube.x;
                        dropping.cy += newCube.y - cube.y;
                        dropping.cz += newCube.h - cube.h;
                    }
                    cube = newCube;
                    EntityManager.SetComponentData(entity, new Translation
                    {
                        Value = new float3
                        {
                            x = cube.x,
                            z = cube.y,
                            y = cube.h
                        }
                    });
                });
            }

            if (inputSystem.GetKey(KeyCode.J))
            {
                CameraControl.cameraHeight -= Time.DeltaTime * 4;
            }
            else if (inputSystem.GetKey(KeyCode.K))
            {
                CameraControl.cameraHeight += Time.DeltaTime * 4;
            }
            else if (inputSystem.GetKey(KeyCode.H))
            {
                CameraControl.cameraAngle += Time.DeltaTime;
            }
            else if (inputSystem.GetKey(KeyCode.L))
            {
                CameraControl.cameraAngle -= Time.DeltaTime;
            }
        }

        private Cube MoveCube(Dropping dropping, Cube cube, float4x4 matrix)
        {
            float4 position = new float4
            {
                x = dropping.cx,
                y = dropping.cy,
                z = dropping.cz,
                w = 1
            };

            float4 newPosition = math.mul(matrix, position);
            return new Cube
            {
                x = cube.x + (int) math.round(newPosition.x - position.x),
                y = cube.y + (int) math.round(newPosition.y - position.y),
                h = cube.h + (int) math.round(newPosition.z - position.z)
            };
        }
    }
}