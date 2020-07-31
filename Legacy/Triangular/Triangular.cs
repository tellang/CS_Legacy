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
    interface ITerm { public int term { get; set; } }
    interface IStatus { public string stat { get; set; } }
    interface INextCardXPos { public int nextCardXPos { get; set; } }
    interface IRandom { public bool Random(int percent); }
    interface IIdentification : IPosition, IStatus { }
    interface ISequence : IPosition, ITerm, ISize { }
    interface ICusor
    {
        public int index { get; set; }
        public CursorFrame cur { get; set; }
        public DeleteCursor dCur { get; set; }
        public FrontFrame source { get; set; }
        public void SetCursor(ConsoleKeyInfo button);
    }
    struct CardInfo : IPosition, IJob
    {
        public int xPos { get; set; }
        public int yPos { get; set; }
        public int lives { get; set; }
        public bool survival { get; set; }
        public string job { get; set; }
    }

    struct MapInfo : IPosition
    {
        public int xPos { get; set; }
        public int yPos { get; set; }
        public string kind { get; set; }
        public string name { get; set; }
        public CardInfo[] opp;
        public bool cleard { get; set; }
    }

    class BaseFrame : ISequence, IStatus, INextCardXPos
    {
        public BaseFrame(int xPos, int yPos, int width, int height, int term)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.inWidth = width;
            this.inHeight = height;
            this.outHeight = inHeight + 2;
            this.outWidth = 2 * (2 + inWidth);
            this.term = term;
            nextCardXPos = outWidth + term;
            this.stat = "None";
        }

        public BaseFrame(ISequence seq)
        {
            this.xPos = seq.xPos;
            this.yPos = seq.yPos;
            this.inWidth = seq.inWidth;
            this.inHeight = seq.inHeight;
            this.outHeight = inHeight + 2;
            this.outWidth = 2 * (2 + inWidth);
            this.term = term;
            nextCardXPos = outWidth + term;
            this.stat = "None";
        }

        public int xPos { get; set; }
        public int yPos { get; set; }
        public int term { get; set; }
        public int inWidth { get; set; }
        public int inHeight { get; set; }
        public int outWidth { get; set; }
        public int outHeight { get; set; }

        public string stat { get; set; }
        public int nextCardXPos { get; set; }

        public int this[int nth] { get { return xPos + nextCardXPos * nth; } }

        public virtual void Frame(int xPos, int yPos)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(" ┏ ");
            for (int i = 0; i < inWidth; i++)
                Console.Write("━ ");
            Console.Write("┓ ");

            for (int j = 1; j <= inHeight; j++)
            {
                Console.SetCursorPosition(xPos, yPos);
                Console.Write("┃ ");
                for (int k = 1; k <= inWidth; k++)
                    Console.Write(" ");

                Console.Write("┃ ");
            }
            Console.SetCursorPosition(xPos, yPos + inHeight + 1);
            Console.Write("┗");
            for (int m = 1; m <= inWidth; m++)
                Console.Write("━ ");
            Console.Write("┛ ");
        }

        public virtual void CleanFrame(int xPos, int yPos)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(" ┏ ");
            for (int i = 0; i < inWidth; i++)
                Console.Write("━ ");
            Console.Write("┓ ");

            for (int j = 1; j <= inHeight; j++)
            {
                Console.SetCursorPosition(xPos, yPos + j);
                Console.Write("┃ ");
                Console.SetCursorPosition(xPos + (inWidth + 1) * 2, yPos + j);
                Console.Write("┃ ");
            }
            Console.SetCursorPosition(xPos, yPos + inHeight + 1);
            Console.Write("┗");
            for (int m = 1; m <= inWidth; m++)
                Console.Write("━ ");
            Console.Write("┛ ");
        }

        public void Frame(int xPos, int yPos, int num)
        {
            for (int i = 0; i < num; i++)
                Frame(xPos + nextCardXPos * num, yPos);
        }

        public virtual void Frame(int nth)
        {
            Frame(xPos + nextCardXPos * nth, yPos);
        }

        public virtual void CleanFrame(int nth)
        {
            CleanFrame(xPos + nextCardXPos * nth, yPos);
        }

        public virtual void Frame()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Frame(xPos, yPos);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    class BackFrame : BaseFrame
    {
        public BackFrame(int xPos, int yPos, int width, int height, int term) : base(xPos, yPos, width, height, term)
        {
            stat = "Back";
        }

        public BackFrame(ISequence seq) : base(seq)
        {
            stat = "Back";
        }

        public override void Frame(int xPos, int yPos)
        {
            base.Frame(xPos, yPos);
            for (int i = 1; i <= inHeight; i++)
            {
                Console.SetCursorPosition(xPos + 2, yPos + i);
                for (int j = 1; j <= inWidth; j++)
                    Console.Write("▩");
            }
        }
    }

    class FrontFrame : BackFrame, IIdentification
    {
        public FrontFrame(int xPos, int yPos, int width, int height, int term) :
        base(xPos, yPos, width, height, term)
        {
            stat = "Front";
        }

        public FrontFrame(int xPos, int yPos, int width, int height, int term, params string[] job) :
        base(xPos, yPos, width, height, term)
        {
            Initialize(ref info, ref job);
        }

        public FrontFrame(ISequence seq) : base(seq)
        {
            stat = "Front";
        }

        public FrontFrame(ISequence sequence, params string[] job) : this(seq)
        {
            Initialize(ref this.info, ref job);
        }

        public CardInfo[] info = new CardInfo[3];

        public void FrameLabel(int xPos, int yPos, string label)
        {
            Console.SetCursorPosition(xPos + (outWidth - label.Length) / 2, yPos + outHeight / 2);
            Console.Write(label);
        }

        public void FrameLabel(int xPos, int yPos, int nth)
        {
            Console.SetCursorPosition(xPos + (outWidth - info[nth].job.Length) / 2, yPos + outHeight / 2);
            Console.Write(info[nth].job);
        }

        public virtual void FrameLabel(int nth)
        {
            Console.SetCursorPosition(xPos + 2, yPos + nth + 1);
            Console.Write(info[nth].job);
        }

        public void FrameLabel(int nth, string label)
        {
            FrameLabel(xPos + nextCardXPos * nth, yPos, label);
        }

        public void SlotLabel(int xPos, int yPos, int nth, string[] label)
        {
            Console.SetCursorPosition(xPos + (outWidth - label[nth].Length) / 2, yPos + outHeight);
            Console.Write(label[nth]);
        }

        public void SlotLabel(int nth, string[] label)
        {
            SlotLabel(xPos + nextCardXPos * nth, yPos, nth, label);
        }

        public virtual void FrameLabel()
        {
            for (int i = 0; i < info.Length; i++)
            {
                Console.SetCursorPosition(xPos + 2, yPos + i + 1);
                Console.Write(info[i].job);
            }
        }

        public override void Frame(int nth)
        {
            base.Frame(nth);
            lives(xPos + nextCardXPos * nth, yPos, nth);
            FrameLabel(xPos + nextCardXPos * nth, yPos, nth);
        }

        public override void Frame()
        {
            base.Frame(nth);
            for (int i = 0; i < info.Length; i++)
            {
                Console.SetCursorPosition(xPos + 2, yPos + i + 1);
                Console.Write(info[i].job);
            }
        }

        public void Resize(string[] nw) //Resize slot's size  
        {
            for (int i = 0; i < nw.Length; i++)
            {
                if (nw[i].Length > inWidth * 2)
                {
                    if ((nw[i].Length - inWidth * 2) % 2 != 0)
                        inWidth += ((nw[i].Length - inWidth * 2) / 2 + 1);
                    else
                        inWidth += (nw[i].Length - inWidth * 2) / 2;
                }

            }
        }
        public void Initialize(ref CardInfo[] bs, ref string[] nw)
        {
            if (nw.Length >= bs.Length)
                Array.Resize<CardInfo>(ref bs, nw.Length);
            Resize(nw);
            outWidth = 2 * (2 + inWidth);
            nextCardXPos = outWidth + term;
            for (int i = 0; i < nw.Length; i++)
            {
                bs[i].job = nw[i];
                bs[i].xPos = xPos + nextCardXPos * i;
                bs[i].yPos = yPos;
            }
        }

        public void Lives(int xPos, int yPos, int nth)
        {
            ConsoleColor bc, fc;
            bc = Console.BackgroundColor;
            fc = Console.ForegroundColor;

            Effect.SetColor(fore: ConsoleColor.Red, Back: bc);
            Console.SetCursorPosition(xPos + 2, yPos + inHeight);
            for (int i = 0; i < info[nth].lives; i++)
                Console.Write("♡");
            Effect.SetColor(fore: fc, Back: bc);
        }
    }
    class EmptyFrame : BaseFrame
    {
        public EmptyFrame(int xPos, int yPos, int width, int height, int term) : base(xPos, yPos, width, height, term)
        {
            stat = "Empty";
        }

        public EmptyFrame(ISequence seq) : base(seq)
        {
            stat = "Empty";
        }

        public override void Frame(int xPos, int yPos)
        {
            for (int i = 0; i < outHeight; i++)
            {
                Console.SetCursorPosition(xPos, yPos + i);
                for (int j = 0; j < outWidth; j++)
                {
                    Console.Write(" ");
                }
            }
        }
    }

    class CursorFrame : BaseFrame
    {
        public CursorFrame(ISequence seq) : base(seq)
        {

        }
        public override void Frame(int xPos, int yPos)
        {
            Effect.SetColor(fore: ConsoleColor.Yellow);
            int xCur, yCur;
            xCur = xPos - 2;
            yCur = yPos - 1;
            Console.SetCursorPosition(xCur, yCur);
            Console.Write(" ┏ ");
            Console.SetCursorPosition(xCur + outWidth + 2, yCur);
            Console.Write("┓ ");
            Console.SetCursorPosition(xCur, yCur + outHeight + 1);
            Console.Write("┗");
            Console.SetCursorPosition(xCur + outWidth + 2, yCur + outHeight + 1);
            Console.Write("┛ ");
            Effect.DefaultColor();
        }

        public override void Frame(int nth)
        {
            Frame(xPos + nextCardXPos * nth, yPos);
        }
    }

    class DeleteCursor : CursorFrame
    {
        public DeleteCursor(ISequence seq) : base(seq) { }

        public override void Frame(int xPos, int yPos)
        {
            int xCur, yCur;
            xCur = xPos - 2;
            yCur = yPos - 1;
            Console.SetCursorPosition(xCur, yCur);
            Console.Write("  ");
            Console.SetCursorPosition(xCur + outWidth + 2, yCur);
            Console.Write("  ");
            Console.SetCursorPosition(xCur, yCur + outHeight + 1);
            Console.Write("  ");
            Console.SetCursorPosition(xCur + outWidth + 2, yCur + outHeight + 1);
            Console.Write("  ");
        }
    }

    class MapCursor : IButton
    {
        public MapCursor (params MapInfo[] maps)
        {
            if(this.maps.Length >= maps.Length)
                Array.Resize<MapInfo>(ref this.maps, maps.Length);
            for (int i = 0; i < maps.Length; i++)
                this.maps[i] = maps[i];   
        }
        public ConsoleKeyInfo button {get; set;}
        public void SetButton () {button = Console.ReadKey(true);}
        public MapInfo[] map = new MapInfo[4];
        public void Move(){}
    }

    class Cursor : IButton
    {
        public Cursor (params ICusor[] frame)
        {
            if (this.frame.Length >= frame.Length)
                Array.Resize<ICusor>(ref this.frame, frame.Length);
            for (int i = 0; i < frame.Length; i++)
                this.frame[i] = frame[i];
        }

        public enum CursorMode {Menu, User, Opp, GlcMu, Battle, Map, Keep, Exit}
        public static int glcOpert { get; set; }
        public static int oLife { get; set; }
        public static int uLife { get; set; }
        public static bool[] unfilpped = new bool[] {true, true, true};
        public static CursorMode cM {get; set;}
        public static bool unGlanced { get; set; }
        public ConsoleKeyInfo button {get; set;}
        public void SetButton () { button = Console.ReadKey(true);}
        public ICusor[] frame = new ICusor[4];
        public BaseFrame bmu;

        public void Move()
        {
            bmu = new BaseFrame(frame[(int)CursorMode.Menu].source);
            switch (cM)
            {
                case CursorMode.Menu:
                    frame[(int)CursorMode.Menu].SetCursor(button);
                    Console.SetCursorPosition(0, 0);
                    switch (cM)
                    {
                    case CursorMode.User:
                        frame[(int)CursorMode.User].cur.Frame(0);
                        break;
                    case CursorMode.GlcMu:
                        frame[(int)CursorMode.GlcMu].source.Frame();
                        Effect.OppositeColor();
                        frame[(int)CursorMode.GlcMu].source.FrameLabel(0);
                        Effect.DefaultColor();
                        break;
                    case CursorMode.Opp:
                        Console.SetCursorPosition(frame[(int)CursorMode.Opp].source.xPos+
                        (frame[(int)CursorMode.Opp].source.term + 
                        frame[(int)CursorMode.Opp].source.outWidth)*3/2 - 12,
                        frame[(int)CursorMode.Opp].source.info[0].yPos -2);
                        Console.Write("Glance Oppertunity: {0}", glcOpert + 1);
                        frame[(int)CursorMode.Opp].cur.Frame(0);
                        break;
                    case CursorMode.Battle:
                        Judgement.Battle(frame[(int)CursorMode.User].source,
                        frame[(int)CursorMode.Opp].source, ref unfilpped);
                        cM = CursorMode.Menu;
                        frame[(int)CursorMode.Menu].cur.Frame(frame[(int)CursorMode.Menu].index);
                        break;
                    }
                    break;

                case CursorMode.User:
                    frame[(int)CursorMode.User].SetCursor(button);
                    if(cM == CursorMode.Menu)
                        frame[(int)CursorMode.Menu].cur.Frame(0);
                    break;
                
                case CursorMode.Opp:
                    frame[(int)CursorMode.Opp].SetCursor(button);

                    switch (cM)
                    {
                        case CursorMode.Menu:
                            frame[(int)CursorMode.Menu].cur.Frame(0);
                            break;
                        case CursorMode.GlcMu:
                            frame[(int)CursorMode.GlcMu].source.Frame();
                            Effect.OppositeColor();
                            frame[(int)CursorMode.GlcMu].source.FrameLabel(0);
                            Effect.DefaultColor();
                            break;
                    }
                    if (glcOpert < 0)
                    {
                        Effect.ShadeColor();
                        bmu.Frame(2);
                        Effect.DefaultColor();
                    }
                    break;
                
                case CursorMode.GlcMu:
                    frame[(int)CursorMode.GlcMu].SetCursor(button);

                    switch (cM)
                    {
                        case CursorMode.Menu:
                            frame[(int)CursorMode.Menu].cur.Frame(0);
                            break;
                        case CursorMode.Opp:
                            Console.SetCursorPosition(frame[(int)CursorMode.Opp].source.xPos +
                            (frame[(int)CursorMode.Opp].source.term +
                            frame[(int)CursorMode.Opp].source.outWidth)*3/2 - 12,
                            frame[(int)CursorMode.Opp].source.info[0].yPos - 2);
                            frame[(int)CursorMode.Opp].cur.Frame(0);
                            break;
                    }
                    break;
                
                default:
                    cM = CursorMode.Menu;
                    break;
            }
            SetButton();
        }
    }

    
}