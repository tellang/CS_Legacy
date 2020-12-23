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
        public bool isSurvival { get; set; }
        public string job { get; set; }
    }

    struct MapInfo : IPosition
    {
        public int xPos { get; set; }
        public int yPos { get; set; }
        public string kind { get; set; }
        public string name { get; set; }
        public CardInfo[] opp;
        public bool isCleared { get; set; }
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

        public virtual void PrintFrame(int xPos, int yPos)
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

        public virtual void PrintCleanFrame(int xPos, int yPos)
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

        public void PrintFrame(int xPos, int yPos, int num)
        {
            for (int i = 0; i < num; i++)
                PrintFrame(xPos + nextCardXPos * num, yPos);
        }

        public virtual void PrintFrame(int nth)
        {
            PrintFrame(xPos + nextCardXPos * nth, yPos);
        }

        public virtual void PrintCleanFrame(int nth)
        {
            PrintCleanFrame(xPos + nextCardXPos * nth, yPos);
        }

        public virtual void PrintFrame()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintFrame(xPos, yPos);

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

        public override void PrintFrame(int xPos, int yPos)
        {
            base.PrintFrame(xPos, yPos);
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

        public FrontFrame(ISequence sequence) : base(sequence)
        {
            stat = "Front";
        }

        public FrontFrame(ISequence sequence, params string[] job) : this(sequence)
        {
            Initialize(ref this.info, ref job);
        }

        public CardInfo[] info = new CardInfo[3];

        public void PrintFrameLabel(int xPos, int yPos, string label)
        {
            Console.SetCursorPosition(xPos + (outWidth - label.Length) / 2, yPos + outHeight / 2);
            Console.Write(label);
        }

        public void PrintFrameLabel(int xPos, int yPos, int nth)
        {
            Console.SetCursorPosition(xPos + (outWidth - info[nth].job.Length) / 2, yPos + outHeight / 2);
            Console.Write(info[nth].job);
        }

        public virtual void PrintFrameLabel(int nth)
        {
            Console.SetCursorPosition(xPos + 2, yPos + nth + 1);
            Console.Write(info[nth].job);
        }

        public void PrintFrameLabel(int nth, string label)
        {
            PrintFrameLabel(xPos + nextCardXPos * nth, yPos, label);
        }

        public void PrintSlotLabel(int xPos, int yPos, int nth, string[] label)
        {
            Console.SetCursorPosition(xPos + (outWidth - label[nth].Length) / 2, yPos + outHeight);
            Console.Write(label[nth]);
        }

        public void PrintSlotLabel(int nth, string[] label)
        {
            PrintSlotLabel(xPos + nextCardXPos * nth, yPos, nth, label);
        }

        public virtual void PrintFrameLabel()
        {
            for (int i = 0; i < info.Length; i++)
            {
                Console.SetCursorPosition(xPos + 2, yPos + i + 1);
                Console.Write(info[i].job);
            }
        }

        public override void PrintFrame(int nth)
        {
            base.PrintFrame(nth);
            Lives(xPos + nextCardXPos * nth, yPos, nth);
            PrintFrameLabel(xPos + nextCardXPos * nth, yPos, nth);
        }

        public override void PrintFrame()
        {
            base.PrintFrame(0);
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
            ConsoleColor backColor, foreColor;
            backColor = Console.BackgroundColor;
            foreColor = Console.ForegroundColor;

            Effect.SetColor(fore: ConsoleColor.Red, back: backColor);
            Console.SetCursorPosition(xPos + 2, yPos + inHeight);
            for (int i = 0; i < info[nth].lives; i++)
                Console.Write("♡");
            Effect.SetColor(fore: foreColor, back: backColor);
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

        public override void PrintFrame(int xPos, int yPos)
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
        public override void PrintFrame(int xPos, int yPos)
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

        public override void PrintFrame(int nth)
        {
            PrintFrame(xPos + nextCardXPos * nth, yPos);
        }
    }

    class DeleteCursor : CursorFrame
    {
        public DeleteCursor(ISequence seq) : base(seq) { }

        public override void PrintFrame(int xPos, int yPos)
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

    /*class MapCursor : IButton
    {
        public MapCursor(params MapInfo[] maps)
        {
            if (this.maps.Length >= maps.Length)
                Array.Resize<MapInfo>(ref this.maps, maps.Length);
            for (int i = 0; i < maps.Length; i++)
                this.maps[i] = maps[i];
        }
        public ConsoleKeyInfo button { get; set; }
        public void SetButton() { button = Console.ReadKey(true); }
        public MapInfo[] map = new MapInfo[4];
        public void Move() { }
    }*/

    class Cursor : IButton
    {
        public Cursor(params ICusor[] frame)
        {
            if (this.frame.Length >= frame.Length)
                Array.Resize<ICusor>(ref this.frame, frame.Length);
            for (int i = 0; i < frame.Length; i++)
                this.frame[i] = frame[i];
        }

        public enum CursorMode { Menu, User, Opp, GlanceMenu, Battle, Map, Keep, Exit }
        public static int glcOpert { get; set; }
        public static int oLife { get; set; }
        public static int uLife { get; set; }
        public static bool[] isUnflipped = new bool[] { true, true, true };
        public static CursorMode cM { get; set; }
        public static bool isUnGlanced { get; set; }
        public ConsoleKeyInfo button { get; set; }
        public void SetButton() { button = Console.ReadKey(true); }
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
                            frame[(int)CursorMode.User].cur.PrintFrame(0);
                            break;
                        case CursorMode.GlanceMenu:
                            frame[(int)CursorMode.GlanceMenu].source.PrintFrame();
                            Effect.OppositeColor();
                            frame[(int)CursorMode.GlanceMenu].source.PrintFrameLabel(0);
                            Effect.DefaultColor();
                            break;
                        case CursorMode.Opp:
                            Console.SetCursorPosition(frame[(int)CursorMode.Opp].source.xPos +
                            (frame[(int)CursorMode.Opp].source.term +
                            frame[(int)CursorMode.Opp].source.outWidth) * 3 / 2 - 12,
                            frame[(int)CursorMode.Opp].source.info[0].yPos - 2);
                            Console.Write("Glance Oppertunity: {0}", glcOpert + 1);
                            frame[(int)CursorMode.Opp].cur.PrintFrame(0);
                            break;
                        case CursorMode.Battle:
                            Judgement.Battle(frame[(int)CursorMode.User].source,
                            frame[(int)CursorMode.Opp].source, ref isUnflipped);
                            cM = CursorMode.Menu;
                            frame[(int)CursorMode.Menu].cur.PrintFrame(frame[(int)CursorMode.Menu].index);
                            break;
                    }
                    break;

                case CursorMode.User:
                    frame[(int)CursorMode.User].SetCursor(button);
                    if (cM == CursorMode.Menu)
                        frame[(int)CursorMode.Menu].cur.PrintFrame(0);
                    break;

                case CursorMode.Opp:
                    frame[(int)CursorMode.Opp].SetCursor(button);

                    switch (cM)
                    {
                        case CursorMode.Menu:
                            frame[(int)CursorMode.Menu].cur.PrintFrame(0);
                            break;
                        case CursorMode.GlanceMenu:
                            frame[(int)CursorMode.GlanceMenu].source.PrintFrame();
                            Effect.OppositeColor();
                            frame[(int)CursorMode.GlanceMenu].source.PrintFrameLabel(0);
                            Effect.DefaultColor();
                            break;
                    }
                    if (glcOpert < 0)
                    {
                        Effect.ShadeColor();
                        bmu.PrintFrame(2);
                        Effect.DefaultColor();
                    }
                    break;

                case CursorMode.GlanceMenu:
                    frame[(int)CursorMode.GlanceMenu].SetCursor(button);

                    switch (cM)
                    {
                        case CursorMode.Menu:
                            frame[(int)CursorMode.Menu].cur.PrintFrame(0);
                            break;
                        case CursorMode.Opp:
                            Console.SetCursorPosition(frame[(int)CursorMode.Opp].source.xPos +
                            (frame[(int)CursorMode.Opp].source.term +
                            frame[(int)CursorMode.Opp].source.outWidth) * 3 / 2 - 12,
                            frame[(int)CursorMode.Opp].source.info[0].yPos - 2);
                            frame[(int)CursorMode.Opp].cur.PrintFrame(0);
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

    class UserFrame : Cursor, ICusor
    {
        public UserFrame(ref FrontFrame source)
        {
            this.source = source;
            this.cur = new CursorFrame(source);
            this.dCur = new DeleteCursor(source);
        }

        public FrontFrame source { get; set; }
        public CursorFrame cur { get; set; }
        public DeleteCursor dCur { get; set; }
        public int index { get; set; }
        private CardInfo tempInfo;
        private bool isGrabbed;
        private int nth;
        public void SetCursor(ConsoleKeyInfo button)
        {
            switch (button.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (index > 0)
                    {
                        dCur.PrintFrame(index--);
                        cur.PrintFrame(index);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (index < source.info.Length - 1)
                    {
                        dCur.PrintFrame(index++);
                        cur.PrintFrame(index);
                    }
                    break;
                case ConsoleKey.Enter:
                    if (source.info[index].isSurvival)
                    {
                        if (isGrabbed)
                        {
                            source.info[nth] = source.info[index];
                            source.info[index] = tempInfo;
                            Effect.DefaultColor();
                            source.PrintFrame(index);
                            source.PrintFrame(nth);
                            isGrabbed = false;
                        }
                        else
                        {
                            Effect.OppositeColor();
                            source.PrintFrame(index);
                            Effect.DefaultColor();
                            isGrabbed = true;
                            tempInfo = source.info[index];
                            nth = index;
                        }
                    }
                    break;
                case ConsoleKey.Escape:
                    if (isGrabbed)
                    {
                        isGrabbed = false;
                        source.PrintFrame(nth);
                    }
                    else
                    {
                        dCur.PrintFrame(index);
                        index = 0;
                        cM = CursorMode.Menu;
                    }
                    break;
                default:
                    break;
            }
            Console.SetCursorPosition(0, 0);
        }
    }

    class MenuFrame : Cursor, ICusor
    {
        public MenuFrame(FrontFrame source)
        {
            this.source = source;
            this.cur = new CursorFrame(source);
            this.dCur = new DeleteCursor(source);
        }

        public FrontFrame source { get; set; }
        public CursorFrame cur { get; set; }
        public DeleteCursor dCur { get; set; }
        public int index { get; set; }
        public void SetCursor(ConsoleKeyInfo button)
        {
            cur.PrintFrame(index);
            switch (button.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (index > 0)
                    {
                        dCur.PrintFrame(index--);
                        cur.PrintFrame(index);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (glcOpert < 0)
                    {
                        if (index < source.info.Length - 2)
                        {
                            dCur.PrintFrame(index++);
                            cur.PrintFrame(index);
                        }
                    }
                    else
                    {
                        if (index < source.info.Length - 1)
                        {
                            dCur.PrintFrame(index++);
                            cur.PrintFrame(index);
                        }
                    }
                    break;
                case ConsoleKey.Enter:
                    switch (index)
                    {
                        case 0:
                            cM = CursorMode.User;
                            dCur.PrintFrame(index);
                            break;
                        case 1:
                            cM = CursorMode.Battle;
                            dCur.PrintFrame(index);
                            break;
                        case 2:
                            if (isUnGlanced)
                            {
                                cM = CursorMode.GlanceMenu;
                                dCur.PrintFrame(index);
                                index = 0;
                            }
                            else
                            {
                                cM = CursorMode.Opp;
                                dCur.PrintFrame(index);
                                index = 0;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case ConsoleKey.Escape:
                    cM = CursorMode.Exit;
                    break;
                default:
                    break;
            }
        }
    }

    class Effect
    {
        public static void SetColor(ConsoleColor back = ConsoleColor.Black, ConsoleColor fore = ConsoleColor.Gray)
        {
            Console.BackgroundColor = back;
            Console.ForegroundColor = fore;
        }
        public static void DefaultColor()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void ShadeColor()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        public static void OppositeColor()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        public static void Blink(BaseFrame frame, int nth, int delay, ConsoleColor f, ConsoleColor b = ConsoleColor.Black)
        {
            ConsoleColor backColor, foreColor;
            backColor = Console.BackgroundColor;
            foreColor = Console.ForegroundColor;

            SetColor(back: b, fore: f);
            frame.PrintFrame(nth);
            Thread.Sleep(delay);
            SetColor(back: backColor, fore: foreColor);
            frame.PrintFrame(nth);
        }

        public static void Blink(BaseFrame frame, int nth, int delay, ConsoleColor f)
        {
            ConsoleColor backColor, foreColor;
            backColor = Console.BackgroundColor;
            foreColor = Console.ForegroundColor;

            SetColor(back: backColor, fore: f);
            frame.PrintFrame(nth);
            Thread.Sleep(delay);
            SetColor(back: backColor, fore: foreColor);
            //frame.PrintFrame(nth);
        }

        public static void Blink(FrontFrame frame, int nth, int delay, string label, ConsoleColor f, ConsoleColor b)
        {
            ConsoleColor backColor, foreColor;
            backColor = Console.BackgroundColor;
            foreColor = Console.ForegroundColor;

            SetColor(back: b, fore: f);
            frame.PrintFrameLabel(nth, label);
            Thread.Sleep(delay);
            SetColor(back: backColor, fore: foreColor);
            frame.PrintFrame(nth);
        }

        public static void Blink(BaseFrame frame, int nth, int delay, ConsoleColor foreStartColor, ConsoleColor backStartColor,
        ConsoleColor foreLastColor, ConsoleColor backLastColor)
        {
            SetColor(back: backStartColor, fore: foreStartColor);
            frame.PrintFrame(nth);
            Thread.Sleep(delay);
            SetColor(back: backLastColor, fore: foreLastColor);
            frame.PrintCleanFrame(nth);
        }
    }

    class GlanceMenuFrame : Cursor, ICusor
    {
        public GlanceMenuFrame (FrontFrame source)
        {

        }
        public FrontFrame source {get; set;}
        public CursorFrame cur { get; set; }
        public DeleteCursor dCur { get; set; }
        public EmptyFrame eFrame { get; set; }
        public int index { get; set; }
        public void SetCursor(ConsoleKeyInfo button) //Need SetButton
        {
            switch (button.Key)
            {
                case ConsoleKey.UpArrow:
                    if (index > 0)
                    {
                        if (button.Key != ConsoleKey.Escape)
                        {
                            source.PrintFrameLabel();
                            Effect.OppositeColor();
                            source.PrintFrameLabel(--index);
                            Effect.DefaultColor();
                        }
                    }break;
                case ConsoleKey.DownArrow:
                    if(index < source.info.Length - 1)
                    {
                        if (button.Key != ConsoleKey.Escape)
                        {
                            source.PrintFrameLabel();
                            Effect.OppositeColor();
                            source.PrintFrameLabel(++index);
                            Effect.DefaultColor();
                        }
                    }break;
                case ConsoleKey.Enter:
                    eFrame.PrintFrame();
                    index = 0;
                    cM= CursorMode.Menu;
                    break;
                default:
                    break;
            }
        }
    }

    class OppFrame : Cursor, ICusor 
    {
        public OppFrame (ref FrontFrame source)
        {
            this.source = source;
            this.cur = new CursorFrame(source);
            this.dCur = new DeleteCursor(source);
        }
        public FrontFrame source {get; set;}
        public CursorFrame cur { get; set; }
        public DeleteCursor dCur { get; set; }
        public int index { get; set; }

        public void SetCursor(ConsoleKeyInfo button)
        {
            switch (button.Key)
            {
                case ConsoleKey.LeftArrow:
                    if(index > 0)
                    {
                        dCur.PrintFrame(index--);
                        cur.PrintFrame(index);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if(index < source.info.Length - 1)
                    {
                        dCur.PrintFrame(index++);
                        cur.PrintFrame(index);
                    }
                    break;
                case ConsoleKey.Enter:
                    if (source.info[index].isSurvival)
                    {
                        int delay = 500;
                        EffectFrame efFrame = new EffectFrame(source);
                        if(isUnflipped[index])
                        {
                            if(glcOpert > -1)
                            {
                                if(Judgement.Random(Judgement.flip[glcOpert]))
                                {
                                    source.PrintFrame(index);
                                    isUnflipped[index] = false;
                                    efFrame.BlinkMessage(index, "Success!", delay, fore: ConsoleColor.Green, 
                                    back: ConsoleColor.Black);
                                }
                                else
                                {
                                    efFrame.BlinkMessage(index, "Failed", delay, fore: ConsoleColor.Red, 
                                    back: ConsoleColor.Black);
                                }
                                isUnGlanced = false;
                            }
                            else
                            {
                                efFrame.BlinkMessage(index, "Disable", delay, fore: ConsoleColor.Magenta, 
                                back: ConsoleColor.Black);
                            }
                            glcOpert -=1;
                            Console.SetCursorPosition(source.xPos + (source.term + source.outWidth)*3/2 - 12,
                            source.info[0].yPos - 2);
                            Console.Write("Glance Oppertunity: {0}", glcOpert + 1);
                            if(glcOpert < 0)
                            {
                                dCur.PrintFrame(index);
                                index = 0;
                                cM = CursorMode.Menu;
                            }
                        }
                        else
                        {
                            efFrame.BlinkMessage(index, "Flipped", delay, fore: ConsoleColor.Yellow, 
                            back: ConsoleColor.Black);
                        }
                    }
                    break;
                case ConsoleKey.Escape:
                    if(isUnGlanced)
                    {
                        dCur.PrintFrame(index);
                        index = 0;
                        cM = CursorMode.GlanceMenu;
                    }
                    else
                    {
                        dCur.PrintFrame(index);
                        index = 0;
                        cM = CursorMode.Menu;
                    }
                    break;
                default:
                    break;
            }
            Console.SetCursorPosition(0, 0);
        }
    }

    class Mapframe // need
    {
        public Mapframe (ref FrontFrame source)
        {

        }
        public FrontFrame source {get; set;}
        public CursorFrame cur { get; set; }
        public DeleteCursor dCur { get; set; }
        public int index { get; set; }
    }

    class UI //need
    {
        public static void SuperiorIcon(int xPos, int yPos)
        {

        }
    }

    class EffectFrame : BaseFrame
    {
        private string[] thifSkill = new string[24] {"構", "機", "顧", "饋", "朽", "鎭", "蘇", "築", 
        "臟", "臝", "臙", "臘", "艪", "聳", "聱", "驌", "驂", "軌", "黑", "俗", "套", "休", "宦", "情"};
        private int[] term = new int [24] {20, 15, 10, 5, 1, 
        1, 1, 1, 1, 1, 
        1, 1, 1, 1, 1, 
        1, 1, 1, 1, 1, 
        1, 1, 1, 1};
        private bool[, ] isUnUsed = new bool[5, 4];

        public EffectFrame (int xPos, int yPos, int width, int height, int term): base(xPos, yPos, width, height, term)
        {
           stat = "Effect";
        }
        
        public EffectFrame (ISequence seq): base(seq)
        {
            stat = "Effect";
        }

        public void Impact (int xPos, int yPos, int delay)
        {
            int rx, ry, i;
            i=0;
            Random rand = new Random();

            for(int y =0; y < 5; y++){ //5x4?
                for (int x = 0; x < 4; x++)
                {
                    isUnUsed[y, x] = true;
                }
            }

            while(i < 12)
            {
                rx = rand.Next(1, 5);
                ry = rand.Next(1, 6);

                while(isUnUsed[ry - 1, rx - 1])
                {
                    if ((rx + ry) < 6)
                    {
                        Cursor(xPos + 2 * (rx + ry), yPos, "★");
                        Thread.Sleep(delay);
                        for (int k = 0; k <= ry; k++)
                        {
                            Cursor(xPos + 2 * (rx + ry - 1 - k), yPos + 1 + k, "★");
                            Cursor(xPos + 2 * (rx + ry - k), yPos + k, "☆");
                            Thread.Sleep(delay);
                            Cursor(xPos + 2 * (rx + ry - k), yPos + i, "  ");
                        }
                    }
                    else
                    {
                        Cursor(xPos + 10, yPos + ry - (5 - rx), "★");
                        Thread.Sleep(delay);
                        for (int k = 0; k <= (4-rx); k++)
                        {
                            Cursor(xPos + 8 - 2*k, yPos + ry - (4 - rx) + k, "★");
                            Cursor(xPos + 10 - 2*k, yPos + ry - (4 - rx) + k - 1, "☆");
                            Thread.Sleep(delay);
                            Cursor(xPos + 10 - 2*k, yPos + ry - (4 - rx) + k - 1, "  ");
                        }
                    }
                    Cursor(xPos + rx * 2, yPos + ry, "●");
                    Thread.Sleep(delay*2);
                    Cursor(xPos + rx * 2, yPos + ry, "○");
                    Thread.Sleep(delay);
                    Cursor(xPos + rx * 2 - 1, yPos + ry, "(○)");
                    Thread.Sleep(delay*2);
                    Cursor(xPos + (rx * - 1)*2, yPos + ry, "(_ _)");
                    Thread.Sleep(delay*2);
                    Cursor(xPos + (rx * - 1)*2, yPos + ry, "         ");

                    isUnUsed[ry - 1, rx - 1] = false;
                    i++;
                }
            }
            Thread.Sleep(500);
        }

        public void Impact(int xPos, int yPos)
        {
            Effect.SetColor(ConsoleColor.Black, ConsoleColor.Cyan);
            Impact(xPos, yPos, 15);
            Effect.DefaultColor();
        }

        public void Impact (int nth)
        {
            Impact(xPos + nextCardXPos * nth, yPos);
        }

        public void Slash (int xPos, int yPos)
        {
            Effect.SetColor(ConsoleColor.Black, ConsoleColor.Red);
            Slash(xPos, yPos, 17);
            Effect.DefaultColor();
        }

        public void Slash (int xPos, int yPos, int delay)
        {
            Cursor(xPos, yPos, "─ ");
            Cursor(xPos, yPos - 1, "└─");
            Cursor(xPos, yPos + 1, "┌─");
            Thread.Sleep(delay);
            Cursor(xPos, yPos, "  ");

            Cursor(xPos, yPos, "─━");
            Cursor(xPos + 2, yPos + 1, "─ ");
            Cursor(xPos + 2, yPos - 1, "─ ");
            Thread.Sleep(delay);
            Cursor(xPos, yPos, "  ");
            for (int i = 0; i < 5; i++)
            {
                Cursor(xPos + i*2, yPos, "─━");
                Cursor(xPos + i*2, yPos + 1, " ─ ");
                Cursor(xPos + i*2, yPos - 1, " ─ ");
                Thread.Sleep(delay);
                Cursor(xPos + i*2, yPos, "  ");
            }
            Cursor(xPos + 8, yPos, "─━");
            Cursor(xPos + 10, yPos + 1, "┴ ");
            Cursor(xPos + 10, yPos - 1, "┬ ");
            Thread.Sleep(delay);
            Cursor(xPos + 8, yPos, "  ");
            Cursor(xPos + 10, yPos, "─ ");
            Thread.Sleep(delay);
            Cursor(xPos + 10, yPos, "  ");

            Thread.Sleep(500);
        }

        public void Slash (int nth)
        {
            Slash(xPos + nextCardXPos * nth, yPos + inHeight/2 + 1);
        }
        
        public void Claw (int xPos, int yPos)
        {
            Effect.SetColor(ConsoleColor.Black, ConsoleColor.Magenta);
            Claw(xPos, yPos);
            Effect.DefaultColor();
        }

        public void Claw (int xPos, int yPos, int delay)
        {
            for (int i = 0; i < 3; i++)
            {
                Cursor(xPos, yPos + i, "＼");
            }
            Thread.Sleep(delay);
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    Cursor(xPos + i * 2, yPos + j + i - 2, "＼");
                }
                Thread.Sleep(delay);
            }
            for(int i=4; i < 7; i++)
            {
                Cursor(xPos + i*2, yPos + i, "＼");
            }
            Thread.Sleep(delay*20);
        }
        public void Claw (int nth)
        {
            Claw(xPos + nextCardXPos * nth, yPos);
        }

        public void Cunjering (int xPos, int yPos, int delay)
        {
            int rx, ry;
            int i = 0;
            Random rand = new Random();

            for(int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    isUnUsed[y, x] = true;
                }
            }

            while(i<20)
            {
                rx = rand.Next(0, 4);
                ry = rand.Next(0, 5);

                while(isUnUsed[ry, rx])
                {
                    Cursor(xPos + 2 + rx * 2, yPos + 1 + ry, thifSkill[i]);
                    isUnUsed[ry, rx] = false;
                    i++;
                    Thread.Sleep(delay*term[i]);
                }
            }
            Thread.Sleep(500);
        }

        public void Cunjering (int xPos, int yPos)
        {
            Effect.SetColor(fore:ConsoleColor.Red, back:ConsoleColor.Yellow);
            Cunjering(xPos, yPos, 20);
            Effect.DefaultColor();
        }

        public void Cunjering(int nth)
        {
            Cunjering(xPos + nextCardXPos * nth, yPos);
        }

        public static void Cursor (int xPos, int yPos, string icon)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.WriteLine(icon);
        }

        public void Message (int xPos, int yPos, string message)
        {
            Cursor(xPos + (outWidth - message.Length)/2, yPos, message);
        }

        public void Cursor (int xPos, int yPos)
        {
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    Cursor(xPos + j*2, yPos + i, "  ");
                }
            }
        }

        public void AttackEffect (FrontFrame atk, int atkNth, int defNth)
        {
            if(atk.info[atkNth].job == "Warrior")
            {
                Slash(defNth);
            }
            else if (atk.info[atkNth].job == "Thif")
            {
                Cunjering(defNth);
            }
            else if (atk.info[atkNth].job == "Sorcerer")
            {
                Impact(defNth);
            }
            else
            {
                Claw(defNth);
            }
        }

        public void BlinkMessage (int nth, string message)
        {
            Message(xPos + nextCardXPos * nth, yPos - 1, message);
        }

        public void BlinkMessage (int nth, string message, int delay, ConsoleColor fore, ConsoleColor back)
        {
            ConsoleColor backC, foreC;
            backC = Console.BackgroundColor;
            foreC = Console.ForegroundColor;

            Effect.SetColor(fore: fore, back: back);
            BlinkMessage(nth, message);
            Thread.Sleep(delay);
            Effect.SetColor(back: back, fore: fore);
            BlinkMessage(nth, "       "); //need adjustment
        }
    }

    class Judgement
    {
        const int atkSuccess = 75; //attack success rate
        const int adventBPAtk = 70; //advantageous battle preempptive attack rate
        const int normalBPAtk = 50; //normal battle preempptive attack rate
        const int penalty = 10;
        const int critical = 15;
        const int delay = 500;
        public static int[] flip = {85, 50, 33}; //card glance success rate, x1, x2, x3 cards

        public static Battle (FrontFrame user, FrontFrame opp, ref bool[] isUnflipped)
        {
            int opponentLife, userLife;
            if (GameManager.IsCheckGameOver(user, opp))
            {

            }
        }

        public static bool Random (int percent)
        {
            int result;
            Random rand = new Random();
            result = rand.Next(0, 100);
            if(result < percent)
                return true;
            else
                return false;
        }

        public static FrontFrame TheOtherJob (FrontFrame cont, FrontFrame user, FrontFrame opp)
        {
            if(cont == user)
                return opp;
            else
                return user;
        }

        public static void CriticalHit (FrontFrame frame, int nth, int percent)
        {
            ConsoleColor black = ConsoleColor.Black;
            ConsoleColor yellow = ConsoleColor.Yellow;
            ConsoleColor white = ConsoleColor.White;

            if(Random(percent))
            {
                Effect.Blink(frame, nth, delay, " Critical!!! ", f: black, b: yellow);
                frame.info[nth].lives -= 2;
                for (int i = 0; i < 3; i++)
                {
                    Effect.Blink(frame, nth, 60, foreStartColor: black, backStartColor: white,
                    foreLastColor: white, backLastColor: black);
                    Thread.Sleep(60);
                    Effect.Blink(frame, nth, 60, foreStartColor: black, backStartColor: yellow,
                    foreLastColor: yellow, backLastColor: black);
                    Thread.Sleep(60);
                }
            } else
            {
                --frame.info[nth].lives;
                for (int i = 0; i < 3; i++)
                {
                    Effect.Blink(frame, nth, 60, foreStartColor: black, backStartColor: white,
                    foreLastColor: white, backLastColor: black);
                    Thread.Sleep(60);
                }
            }
            if (frame.info[nth].lives <= 0)
                frame.info[nth].isSurvival = false;
            Effect.DefaultColor();
        }

        public static FrontFrame Triumpher (FrontFrame user, int nthUser, FrontFrame opp, int nthOpp)
        {

        }

        public static FrontFrame Loser (FrontFrame user, int nthUser, FrontFrame opp, int nthOpp)
        {

        }

        public static string Superior (FrontFrame user, int nthUser, FrontFrame opp, int nthOpp)
        {

        }

        public static FrontFrame FindTriumpher (FrontFrame superior, int nthSuperior, ConsoleColor superiorColor,
        FrontFrame inferior, int nthInferior, ConsoleColor inferiorColor, int superiorAtk, int superiorCritical,
        int inferiorAtk, int inferiorCritical)
        {
            
        }

        public static FrontFrame FindTriumpher (FrontFrame superior, int nthSuperior, ConsoleColor superiorColor,
        FrontFrame inferior, int nthInferior, ConsoleColor inferiorColor, int percent)
        {
            
        }


    }

    class GameManager
    {
        public static bool IsCheckGameOver (CardInfo cardInfo)
        {
            if(cardInfo.isSurvival)
                return false;
            else   
                return true;
        }

        public static bool IsCheckGameOver (FrontFrame user, FrontFrame opp)
        {
            if (Array.TrueForAll<CardInfo>(user.info, IsCheckGameOver) || 
            Array.TrueForAll<CardInfo>(opp.info, IsCheckGameOver))
                return true;
            else
                return false;
        }

        public void Shuffle (string[] before, ref CardInfo[] after)
        {

        }

        public void Zeroize (ref bool[] isUnFlipped)
        {

        }

        public void Zeroize (ref CardInfo[] cardInfo)
        {

        }

        public void Zeroize (FrontFrame user, FrontFrame opponent)
        {

        }

        public void PrintFrame (FrontFrame menu, FrontFrame user, FrontFrame opponent, BackFrame opponentBack)
        {

        }

        public void NewGame()
        {
            
        }
    }

    class MainApp
    {
        
    }
}