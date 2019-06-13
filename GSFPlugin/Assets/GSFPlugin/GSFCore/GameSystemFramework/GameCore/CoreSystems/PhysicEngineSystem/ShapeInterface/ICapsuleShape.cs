using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Physics
{
    public interface ICapsuleShape
    {
        float Radius { get; }
        float Height { get; }

        void SetSize(float radius, float height);
    }
}
