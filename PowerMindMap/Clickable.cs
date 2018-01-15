using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindNoderPort
{
    public interface Clickable
    {
        void setHovered(bool active);
        void SetText(String newtext);
        void UpdateWidth();
        //Clickable ContainsNode(int x, int y);
        bool Contains(int x, int y);
        void DrawRepresentation();
        void updateRepresentation();
        
    }
}
