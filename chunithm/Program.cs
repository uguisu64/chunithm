using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace chunithm
{
    class Program
    {
        static List<TeisuData> teisuDatas = new List<TeisuData>();
        public struct GameData
        {
            public string title;
            public float teisu;
            public int score;
            public int fcaj;
        }
        public struct TeisuData
        {
            public float teisu;
            public string title;
        }
        static void Main(string[] args)
        {


            string path;
            Console.WriteLine("読み込むファイルのパスを入力");
            path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("そのパスは無効です。");
                return;
            }

            StreamReader sr1 = File.OpenText("C:/CHUNITHM/TeisuTitle.txt");
            while (sr1.Peek() > -1)
            {
                string s;
                s = sr1.ReadLine();
                teisuDatas.Add(ReadTeisuData(s));
            }

            StreamReader sr = File.OpenText(path);

            int i = 0;
            GameData[] gameDatas = new GameData[teisuDatas.Count];
            bool first = true;
            bool fcajFlg = false;
            while (sr.Peek() > -1)
            {
                string s;
                s = sr.ReadLine();
                s = s.Trim();

                if (s.StartsWith("<div class=\"music_title\">"))
                {
                    s = Extraction(s);
                    Console.Write(s);
                    

                    if (first)
                    {
                        gameDatas[i].title = s;
                        first = false;
                    }
                    else
                    {
                        if (fcajFlg == false)
                        {
                            gameDatas[i].fcaj = 0;
                        }

                        i++;
                        gameDatas[i].title = s;
                        fcajFlg = false;
                    }

                    float t = FindTeisu(s);
                    if (t == 0)
                    {
                        Console.WriteLine(s + "が見つかりません。");
                        Console.WriteLine("何かキーを押して下さい");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        gameDatas[i].teisu = t;
                    }
                }
                if (s.StartsWith("HIGH SCORE：<span class=\"text_b\">"))
                {
                    s = Extraction(s);
                    Console.WriteLine(highScore(s));

                    gameDatas[i].score = highScore(s);
                }
                if (s == "<!-- ◆オールジャスティス -->")
                {
                    fcajFlg = true;
                    gameDatas[i].fcaj = 2;
                }
                if (s == "<!-- ◆フルコンボ -->")
                {
                    fcajFlg = true;
                    gameDatas[i].fcaj = 1;
                }
            }
            if (!fcajFlg)
            {
                gameDatas[i].fcaj = 0;
            }


            FileStream fs = new FileStream("C:/CHUNITHM/" + DateTime.Now.ToString("yyyy MM dd") + ".txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            float ops = 0;
            float maxops = 0;
            int rironti = 0;
            for(int j = 0; j <= i; j++)
            {
                sw.WriteLine(gameDatas[j].title);
                sw.WriteLine(" 譜面定数:" + gameDatas[j].teisu + "  ハイスコア:" + gameDatas[j].score);
                float op = OverPower(gameDatas[j]);
                float maxop = gameDatas[j].teisu * 5 + 15;
                ops += op;
                maxops += maxop;
                sw.WriteLine(" " + op + "/" + maxop + "  " + op / maxop * 100 + "%");
                sw.Flush();
                if (gameDatas[j].score == 1010000)
                {
                    rironti++;
                }
            }

            sw.WriteLine("合計");
            sw.WriteLine(" " + ops + "/" + maxops + "  " + ops / maxops * 100 + "%");
            sw.Write("理論値:" + rironti);
            sw.Flush();
        }
        static string Extraction(string s)
        {
            string name = "";
            int i;
            for (i = 0; i < s.Length; i++)
            {
                if (s[i] == '>')
                {
                    break;
                }
            }
            for (i++; i < s.Length; i++)
            {
                if (s[i] == '<')
                {
                    break;
                }
                else
                {
                    name += s[i];
                }
            }

            return name;
        }
        static int highScore(string s)
        {
            string score = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ',')
                {
                    continue;
                }
                score += s[i];
            }
            return int.Parse(score);
        }
        static TeisuData ReadTeisuData(string s)
        {
            TeisuData data;
            string teisu = "";
            string title = "";

            int i;
            for (i = 0; i < s.Length; i++)
            {
                if (s[i] == ',')
                {
                    break;
                }
                teisu += s[i];
            }
            for (i++; i < s.Length; i++)
            {
                title += s[i];
            }

            data.title = title;
            data.teisu = float.Parse(teisu);

            return data;
        }
        static float FindTeisu(string title)
        {
            for (int i = 0; i < teisuDatas.Count; i++)
            {
                if (title == teisuDatas[i].title)
                {
                    return teisuDatas[i].teisu;
                }
            }
            return 0;
        }
        static float OverPower(GameData data)
        {
            float op = 0;
            if (data.score == 1010000)
            {
                op = data.teisu * 5 + 15;
            }
            else
            {
                if (data.score >= 1007500)
                {
                    op = data.teisu * 5 + (data.score - 1007500) * 0.0015f + 10;
                }
                else if (data.score >= 1005000)
                {
                    op = data.teisu * 5 + (data.score - 1005000) * 0.001f + 7.5f;
                }
                else if (data.score >= 1000000)
                {
                    op = data.teisu * 5 + (data.score - 1000000) * 0.0005f + 5;
                }
                else if (data.score >= 975000)
                {
                    op = data.teisu * 5 + (data.score - 975000) * 0.0002f;
                }

                if (data.fcaj == 2)
                {
                    op += 1;
                }
                else if (data.fcaj == 1)
                {
                    op += 0.5f;
                }
            }

            return op;
        }
    }
}
