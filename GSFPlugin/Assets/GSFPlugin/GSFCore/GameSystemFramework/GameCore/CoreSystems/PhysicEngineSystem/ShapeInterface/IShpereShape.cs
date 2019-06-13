using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Physics
{
    public interface ISphereShape
    {
        float Radius { get; }

        void SetSize(float radius);
    }
}
