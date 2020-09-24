using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using XRL.Messages;
using XRL.World;

namespace XRL.UI
{
    public delegate ScreenBuffer acegiak_ScreenBufferMaker (ScreenBuffer Buffer, int CharCode);

	public class acegiak_CustomPopup:Popup
	{
        public static string CustomRender(acegiak_ScreenBufferMaker sbMaker,  int width, int height)
        {
            GameManager.Instance.PushGameView("Popup:AskString");
            Loading.SetHideLoadStatus(hidden: true);
            string Return = "";
            int StartingLine = 0;
            ScrapBuffer.Copy(TextConsole.CurrentBuffer);
            ScrapBuffer2.Copy(TextConsole.CurrentBuffer);


            ScreenBuffer Lines = ScreenBuffer.create(width, height);

            int c = (int)'_';
            while (c != 13)
            {
                ScrapBuffer.Copy(ScrapBuffer2);
                Lines = sbMaker(Lines, c);
                RenderCharBlock(Lines, "[Enter to confirm]", "[&WEnter&y to confirm]", ScrapBuffer, StartingLine);
                _TextConsole.DrawBuffer(ScrapBuffer, null, true);

                c = Keyboard.getch();

                if (Keyboard.vkCode == Keys.MouseEvent && Keyboard.CurrentMouseEvent.Event.StartsWith("SubmitString:"))
                {
                    Return = Keyboard.CurrentMouseEvent.Event.Substring(Keyboard.CurrentMouseEvent.Event.IndexOf(':') + 1);

                        Keyboard.ClearInput();
                        GameManager.Instance.PopGameView(true);
                        _TextConsole.DrawBuffer(ScrapBuffer2,null,true);
                        Loading.SetHideLoadStatus(hidden: false);
                        return Return;
                    
                }

                if (Keyboard.vkCode != Keys.MouseEvent)
                {
                    char asChar = (char) c;
                    if (Char.IsDigit(asChar) || Char.IsLetter(asChar) || Char.IsPunctuation(asChar) || Char.IsSeparator(asChar) || Char.IsSymbol(asChar) || c == ' ')
                    {
                        Return += asChar;
                    }
                }

                if (Keyboard.vkCode == Keys.Back )
                {
                    if (Return.Length > 0) Return = Return.Substring(0, Return.Length - 1);
                }

                if (Keyboard.vkCode == Keys.Escape || (Keyboard.vkCode == Keys.MouseEvent && Keyboard.CurrentMouseEvent.Event == "RightClick")) 
                {
                    
                        Keyboard.ClearInput();
                        GameManager.Instance.PopGameView(true);
                        _TextConsole.DrawBuffer(ScrapBuffer2, null, true);
                        Loading.SetHideLoadStatus(hidden: false);
                        return ""; //escape 
                    
                }

                if( (Keyboard.vkCode == Keys.MouseEvent && Keyboard.CurrentMouseEvent.Event == "LeftClick"))
                {
                    break;
                }

//                if (c == (int)KeyCode.CrDown || c == '2') StartingLine++;
//                if (c == (int)KeyCode.CrUp || c == '8') StartingLine--;
//                if (c == (int)KeyCode.PgDown) StartingLine += 23;
//                if (c == (int)KeyCode.PgUp) StartingLine -= 23;
//                if (StartingLine < 0) StartingLine = 0;
            }

            Keyboard.ClearInput();
            GameManager.Instance.PopGameView(true);
            _TextConsole.DrawBuffer(ScrapBuffer2, null, true);
            Loading.SetHideLoadStatus(hidden: false);

            return Return;
        }


        public static int RenderCharBlock(ScreenBuffer Lines, string BottomLineNoFormat, string BottomLine, ScreenBuffer Buffer, int StartingLine, int MinWidth = -1, int MinHeight = -1, string Title = null, bool BottomLineRegions = false )
        {
            int Width = 22;

            if (Title != null)
            {
                string StrippedTitle = ConsoleLib.Console.ColorUtility.StripFormatting(Title);
                if (MinWidth < StrippedTitle.Length + 2) MinWidth = StrippedTitle.Length + 2;
            }
            if( MinWidth > - 1 ) Width = MinWidth;

            // for (int x = 0; x < Lines.Height; x++)
            // {
            //     string Stripped = ConsoleLib.Console.ColorUtility.StripFormatting(Lines[x]);
            //     if (Stripped.Length + 2 >= Width) Width = Stripped.Length + 2;
            // }

            int HOffset = Width / 2;
            int Offset = Lines.Height / 2;

            HOffset++;

            if (MinHeight > -1) Offset = MinHeight;
            if (Offset < 2) Offset = 2;

            int Top = 10 - Offset;
            int Bottom = Top + Lines.Height + 3;
            int Left = 40 - HOffset;
            int Right = 40 + HOffset;

            if (Left < 0) Left = 0;
            if (Right > 79) Right = 79;
            if (Top < 0) Top = 0;
            if (Bottom > 24) Bottom = 24;

            if (Right < MinWidth - 1) Right = MinHeight - 1;
            if (Bottom < MinHeight - 1) Bottom = MinHeight - 1;

            Buffer.Fill(Left, Top, Right, Bottom, ' ', ConsoleLib.Console.ColorUtility.MakeColor(TextColor.Black, TextColor.Black));
            Buffer.ThickSingleBox(Left, Top, Right, Bottom, ConsoleLib.Console.ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));
            
            if( Title != null )
            {
                Buffer.Goto(Left + 1, Top);
                Buffer.Write(Title);
            }

            for (int x = 0; x < Lines.Height; x++)
            {
                Buffer.Goto(Left + 2, Top + 2 + x - StartingLine);

                if (StartingLine > 0 && x == StartingLine)
                {
                    Buffer.Write("<up for more...>");
                }
                else
                    if (Top + 2 + x - StartingLine == 23)
                    {
                        Buffer.Write("<down for more...>");
                    }
                    else
                    {
                        for(int i = 0; i < Lines.Width; i++){
                            if(Lines[i,x] != null && Lines[i,x].Tile != null){
                                //IPart.AddPlayerMessage("Tile:"+Lines[i,x].Tile);
                            
                            Buffer[Left+2+i,Top+2+x] = new ConsoleChar();
                            Buffer[Left+2+i,Top+2+x].Char = Lines[i,x].Char;
                            Buffer[Left+2+i,Top+2+x].Tile = Lines[i,x].Tile;
                            Buffer[Left+2+i,Top+2+x].SetForeground('y');
                            Buffer[Left+2+i,Top+2+x].SetBackground('k');
                            Buffer[Left+2+i,Top+2+x].TileLayerBackground[0] = ConsoleLib.Console.ColorUtility.ColorMap['r'];
                            }
                            // Write(Buffer,Lines[i,x]);
                        }
                    }
            }

            int Spacing = HOffset * 2;
            Spacing -= BottomLineNoFormat.Length;
            Spacing /= 2;
            Spacing++;
            Buffer.Goto(Left + Spacing, Bottom);
            Buffer.Write(BottomLine);

            if (BottomLineRegions)
            {
                GameManager.Instance.ClearRegions();
                List<int> RegionPlaces = new List<int>();
                RegionPlaces.Add(0);
                for (int x = 0; x < BottomLineNoFormat.Length; x++)
                {
                    if (BottomLineNoFormat[x] == ' ')
                    {
                        RegionPlaces.Add(x);
                    }
                }
                RegionPlaces.Add(BottomLineNoFormat.Length);
                for( int x=0;x<RegionPlaces.Count-1;x++ )
                {
                    GameManager.Instance.AddRegion(Left +Spacing + RegionPlaces[x], Bottom-1, Left + Spacing + RegionPlaces[x+1], Bottom+1, "LeftOption:"+(x+1), "RightOption:"+(x+1) );
                }
            }

            return Lines.Height;
        }
        
        // public static void Write(ScreenBuffer Buffer, ConsoleChar c)
		// {
		// 	if (Buffer._X < Buffer.Width && Buffer._X >= 0 && Buffer._Y < Buffer.Height && Buffer._Y >= 0)
		// 	{
		// 		Buffer[Buffer._X, Buffer._Y] = c;
		// 	}
		// 	Buffer.X++;
		// 	if (Buffer.X >= Buffer.Width)
		// 	{
		// 		Buffer.X = 0;
		// 		Buffer.Y++;
		// 	}
		// 	if (Buffer.Y >= Buffer.Height)
		// 	{
		// 		Buffer.Y = 0;
		// 	}
		// }
    }
}