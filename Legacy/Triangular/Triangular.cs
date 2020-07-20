using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Triangular
{
    interface IPosition
    {
        public int xPos { get; set; }
        public int yPos { get; set; }
    }
    interface IDictionary
    {
        public int sort { get; set; }
        public int level { get; set; }
        public int index { get; set; }
    }
    interface IInsideSize
    {
        public int inWidth { get; set; }
        public int inHeight { get; set; }
    }
    interface IOutsideSize
    {
        public int outWidth { get; set; }
        public int outHeight { get; set; }
    }
    interface ISize : IInsideSize, IOutsideSize { }
    interface IDelay { public int delay { get; set; } }
    interface IButton
    {
        public ConsoleKeyInfo button { get; set; }
        public void SetButton();
    }
    interface IJob { public string job { get; set; } }
    interface ITerm{ public int term { get; set; }}
    interface IStatus{public string stat{get; set;}}
    interface INextCardXPos{public int nextCardXPos { get; set; }}
    interface IRandom { public bool Random(int percent);}
    interface IIdentification : IPosition, IStatus{}
    interface ISequence : IPosition, ITerm, ISize{}
    interface ICusor{
        public int index { get; set; }
        public CursorFrame cur {get; set;}
        public DeleteCursor dCur {get; set;}
        public FrontFrame source {get; set;}
        public void SetCursor(ConsoleKeyInfo button);
    }
}