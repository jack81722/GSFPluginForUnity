using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore
{
    public interface IGSVisitor
    {
        void GetGSList(List<GameSourceAdapter> adapters);
    }
}
