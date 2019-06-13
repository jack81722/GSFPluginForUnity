using GameSystem.GameCore.SerializableMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Physics
{
    public interface IBoxShape
    {
        Vector3 HalfSize { get; }

        void SetSize(Vector3 halfSize);
    }

    
}
