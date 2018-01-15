using MindNoderPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMindMap
{
    public interface ActionStackInterface
    {
        void AddAction(MindNodeAction action);
        void UndoLast();
        void RedoLast();
    }
}
