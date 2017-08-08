using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace NppFormatPlugin.DpTools
{
    public class DpTools
    {
        protected const string c_NewLine = "\r\n";
        protected const string c_HorizontalSpace = "\t";

        protected const char c_OpeningTag = '<';
        protected const char c_ClosingTag = '>';

        protected string m_unformatedString;

        // State variables
        protected int m_ColumnCount = 0;
        protected char m_LastControlCharacter = char.MinValue;
        protected bool m_IsFirstOpening = true;

        public DpTools(string unformatedString)
        {
            this.m_unformatedString = unformatedString;
        }

        public void Load(string unformattedString)
        {
            this.m_unformatedString = unformattedString;

            this.m_ColumnCount = 0;
            this.m_LastControlCharacter = char.MinValue;
            this.m_IsFirstOpening = true;
        }

        public string Format()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            StringBuilder formatedStringBuilder = new StringBuilder();
            char currentChar;
            string toAppend = string.Empty;

            for (int i = 0; i < m_unformatedString.Length; ++i)
            {
                currentChar = m_unformatedString[i];
                toAppend = string.Empty;

                if (this.IsOpeningCharacter(currentChar))
                {
                    // erstes <
                    if (m_IsFirstOpening)
                    {
                        // erster '<' -> kein NewLine!
                        m_IsFirstOpening = false;

                        toAppend = Convert.ToString(currentChar);
                    }
                    // nicht erstes <
                    else
                    {
                        // was war davor
                        if (m_LastControlCharacter == c_OpeningTag)
                        {
                            // nur wenn < nach < kommt dann weiter ruecken
                            this.m_ColumnCount += 1;
                        }

                        toAppend = c_NewLine + this.CreateHorizontalSpace(this.m_ColumnCount) + Convert.ToString(currentChar);
                    }

                    this.m_LastControlCharacter = c_OpeningTag;
                }
                else if (this.IsClosingCharacter(currentChar))
                {
                    // closing >
                    if (m_LastControlCharacter == c_OpeningTag)
                    {
                        // > nach <
                        toAppend = Convert.ToString(currentChar);
                    }
                    else
                    {
                        // > nach >
                        this.m_ColumnCount -= 1;

                        toAppend = c_NewLine + this.CreateHorizontalSpace(this.m_ColumnCount) + Convert.ToString(currentChar);
                    }

                    this.m_LastControlCharacter = c_ClosingTag;
                }
                else
                {
                    toAppend = Convert.ToString(currentChar);
                }

                formatedStringBuilder.Append(toAppend);
            }

            stopWatch.Stop();

            return formatedStringBuilder.ToString();
        }

        protected bool IsControlCharacter(char c)
        {
            return (c == c_OpeningTag) || (c == c_ClosingTag);
        }

        protected bool IsOpeningCharacter(char c)
        {
            return (c == c_OpeningTag);
        }

        protected bool IsClosingCharacter(char c)
        {
            return (c == c_ClosingTag);
        }

        protected string CreateHorizontalSpace(int horizontalCount)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < horizontalCount; ++i)
            {
                sb.Append(c_HorizontalSpace);
            }

            return sb.ToString();
        }
    }
}
