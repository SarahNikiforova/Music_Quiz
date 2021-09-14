using System;
using System.Collections.Generic;
using System.IO;

namespace Nikiforova_lab1
{  //класс Тест
    class Test
    {
        //списки с информацией 
        List<String> questions=new List<String>();
        List<List<String>> answers=new List<List<String>>();
        List<List<int>> correctAnswer=new List<List<int>>();
        List<int> scores=new List<int>();
        List<int> numListAskedQuestions = new List<int>();
        List <List<int>> userAnswersList = new List <List<int>>();

        
        int sumScores=0;  //итоговый балл
        int answersReq = 0;  //кол-во вопросов для теста
        int crntAnswer = 0;  //текущий счетчик вопросов
        String info;
        String pathFile;

        //метод
       public Test(String path, int answersReq, String info)
        {
           //доступ к информации
            this.info = info;
            this.answersReq=answersReq;
            this.pathFile = path;
            
            try
            {
                //чтение файла
                using (StreamReader read = new StreamReader(path))
                    while (!read.EndOfStream) 
                    {
                        
                        string[] param = read.ReadLine().Trim().Split(new char[] { ';' });
                        questions.Add(param[0].Trim());
                        scores.Add(int.Parse(param[1].Trim()));
                        string[] corrAnswers = param[2].Trim().Split(new char[] { ',' });
                        List<int> corrAnswersList = new List<int>();
                        for (int i = 0; i < corrAnswers.Length; i++)
                            corrAnswersList.Add(int.Parse(corrAnswers[i].Trim()));
                        correctAnswer.Add(corrAnswersList);

                        List<String> answersList = new List<String>();
                        for (int i = 3; i < param.Length-1; i++)
                            answersList.Add(param[i].Trim());
                        answers.Add(answersList);
                    }

                
                if (answersReq > questions.Count) 
                    throw new Exception("Введите от 1 до 10");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Во время чтения файла произошла ошибка! Приложение будет закрыто! "+
                                  "Подробности: " + ex.Message);
                Console.ReadKey();
               // System.Environment.Exit(1);
            }
        }

       public bool RunTesting()
        {
            
            Console.WriteLine("Вопросы:");

            try
            {
                Random rnd = new Random();
                
                while (crntAnswer < answersReq)
                {
                    int numRnd=0;
                    while(true)
                    {
                        numRnd=rnd.Next(0,questions.Count);
                        if (!numListAskedQuestions.Exists(e=> e.Equals(numRnd))) break; //проверка: был уже вопрос или нет 
                    }
                    numListAskedQuestions.Add(numRnd);
                    Console.WriteLine(numRnd.ToString()+". "+ questions[numRnd]);
                    for (int i = 0; i < answers[numRnd].Count; i++)
                        Console.WriteLine((i+1).ToString()+". "+answers[numRnd][i]);
                    
                    Console.WriteLine("Введите Ваш ответ:");
                    String userAnswer=Console.ReadLine().Trim();
                    String[] userAnswerArr = userAnswer.Split(new char[] { ',' });

                    //проверяем и запоминаем ответы
                    bool ok=true; List <int> answ = new List<int>();
                    for (int i = 0; i < userAnswerArr.Length; i++)
                    {
                        answ.Add(int.Parse(userAnswerArr[i].Trim()));
                        if (!correctAnswer[numRnd].Exists(e => e.Equals(int.Parse(userAnswerArr[i].Trim())))) //проверка на верные ответы
                            ok = !ok;
                    }
                    userAnswersList.Add(answ);
                    // подсчет баллов
                    if (ok) sumScores += scores[numRnd];   
                    crntAnswer++;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Во время прохождения теста произошла ошибка! Приложение будет закрыто! "+
                                  "Подробности: " + ex.Message);
                Console.ReadKey();
              //  System.Environment.Exit(2);
            }
            
            try
            {   //запись в файл
                using (StreamWriter sw = new StreamWriter(pathFile + "-" + info  + ".txt"))
                {
                    sw.WriteLine("Информация о пользователи: " + this.info);
                  
                    
                    //посчитаем максимальное кол-во баллов
                    int maxScores=0;
                    for(int i=0;i<numListAskedQuestions.Count;i++)
                        maxScores+=scores[numListAskedQuestions[i]];

                    sw.WriteLine("Итоговый балл: " + sumScores + " из " + maxScores);
                
                    sw.WriteLine("Вопросы и ответы пользователя:");
                    for(int i=0;i<numListAskedQuestions.Count;i++)
                    {
                        sw.WriteLine("Вопрос:" + (i + 1).ToString() + ". " + questions[numListAskedQuestions[i]] + 
                                     "Максимальный балл:"+scores[numListAskedQuestions[i]]);
                        
                        for (int j=0;j<userAnswersList[i].Count;j++)
                        {
                            String aswersRead="";
                            for (int m = 0; m < answers[numListAskedQuestions[i]].Count; m++)
                                aswersRead = answers[numListAskedQuestions[i]][m];
                            sw.WriteLine(userAnswersList[i][j].ToString()+". "+aswersRead);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Во время записи результатов теста произошла ошибка! Приложение будет закрыто! "+
                                  "Подробности: " + ex.Message);
                Console.ReadKey();
               // System.Environment.Exit(3);
            }

            return true;
          }
    }

    class Program
    {
        static void Main(string[] args)
        {
           /* if (args.Length<2)
            {
                Console.WriteLine("Использование: lab1.exe \"Путь к файлу csv\" <кол-во вопросов для тестирования>");
                Console.ReadKey();
                System.Environment.Exit(4);
            }*/

            Console.Write("Введите фио: ");
            String fio=Console.ReadLine();
            
            int cntQuestions=0;
            try
            {
                cntQuestions = int.Parse(args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Во время преобразования входного параметра <кол-во вопросов для тестирования> "+
                                  "произошла ошибка! Приложение будет закрыто! " +"Подробности: " + ex.Message);
                Console.ReadKey();
               // System.Environment.Exit(5);
            }

            try
            {
                Test testCore = new Test(args[0], int.Parse(args[1]), fio);
                testCore.RunTesting();

                Console.WriteLine("Вы завершили тест! Поздравляем!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Во время прохождения теста возникла ошибка! Приложение будет закрыто! " +
                                  "Подробности: " + ex.Message);
                Console.ReadKey();
               // System.Environment.Exit(6);
            }
            
        }
    }
}
