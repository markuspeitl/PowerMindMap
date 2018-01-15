using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindNoderPort
{
    public class ButtonManager
    {

        public ButtonManager()
        {

        }

        public bool ButtonCLicked(Button clickbutton)
        {
            if (clickbutton.Equals(Button.ADD))
            {
                if (GlobalNodeHandler.adding)
                {
                    GlobalNodeHandler.adding = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.adding = true;
                    GlobalNodeHandler.selectedButton = Button.ADD;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.CONNECT))
            {
                if (GlobalNodeHandler.connecting)
                {
                    GlobalNodeHandler.connecting = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.connecting = true;
                    GlobalNodeHandler.selectedButton = Button.CONNECT;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.DISCONN))
            {
                if (GlobalNodeHandler.disconnecting)
                {
                    GlobalNodeHandler.disconnecting = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.disconnecting = true;
                    GlobalNodeHandler.selectedButton = Button.DISCONN;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.DELETE))
            {
                if (GlobalNodeHandler.deleting)
                {
                    GlobalNodeHandler.deleting = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.deleting = true;
                    GlobalNodeHandler.selectedButton = Button.DELETE;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.MOVE))
            {
                if (GlobalNodeHandler.moving)
                {
                    GlobalNodeHandler.moving = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.moving = true;
                    GlobalNodeHandler.selectedButton = Button.MOVE;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.TRANSFORM))
            {
                if (GlobalNodeHandler.transforming)
                {
                    GlobalNodeHandler.transforming = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.transforming = true;
                    GlobalNodeHandler.selectedButton = Button.TRANSFORM;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.SELECT))
            {
                if (GlobalNodeHandler.selecting)
                {
                    GlobalNodeHandler.selecting = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.selecting = true;
                    GlobalNodeHandler.selectedButton = Button.SELECT;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.INK))
            {
            }
            else if (clickbutton.Equals(Button.REDO))
            {
            }
            else if (clickbutton.Equals(Button.UNDO))
            {
            }
            else if (clickbutton.Equals(Button.COPY))
            {
                if (GlobalNodeHandler.copy)
                {
                    GlobalNodeHandler.copy = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.copy = true;
                    GlobalNodeHandler.selectedButton = Button.COPY;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.PASTE))
            {
                if (GlobalNodeHandler.paste)
                {
                    GlobalNodeHandler.paste = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.paste = true;
                    GlobalNodeHandler.selectedButton = Button.PASTE;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.CUT))
            {
                if (GlobalNodeHandler.cut)
                {
                    GlobalNodeHandler.cut = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.cut = true;
                    GlobalNodeHandler.selectedButton = Button.CUT;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.ADDLABEL))
            {
                if (GlobalNodeHandler.placelabel)
                {
                    GlobalNodeHandler.placelabel = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.placelabel = true;
                    GlobalNodeHandler.selectedButton = Button.ADDLABEL;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.JUMPIN))
            {
                if (GlobalNodeHandler.jumping)
                {
                    GlobalNodeHandler.jumping = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.jumping = true;
                    GlobalNodeHandler.selectedButton = Button.JUMPIN;
                    return true;
                }
            }
            else if (clickbutton.Equals(Button.COLORNODE))
            {
                if (GlobalNodeHandler.coloring)
                {
                    GlobalNodeHandler.coloring = false;
                    GlobalNodeHandler.selectedButton = Button.NONE;
                    return false;
                }
                else if (GlobalNodeHandler.selectedButton.Equals(Button.NONE))
                {
                    GlobalNodeHandler.coloring = true;
                    GlobalNodeHandler.selectedButton = Button.COLORNODE;
                    return true;
                }
            }

            return false;
        }

    }

    public enum Button
    {
        NONE,
        ADD,
        CONNECT,
        DISCONN,
        DELETE,
        MOVE,
        TRANSFORM,
        SELECT,
        INK,
        UNDO,
        REDO,
        COPY,
        CUT,
        PASTE,
        ADDLABEL,
        JUMPIN,
        COLORNODE
    }
}
