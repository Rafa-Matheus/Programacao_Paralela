using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; // Para thread e sleep
using System.Threading.Tasks;

namespace ProgramacaoParalela
{
    internal class ThreadParametersForTheExampleWithT5
    {
        public int StartCounter { get; set; }
        public string Name { get; set; }

        public ThreadParametersForTheExampleWithT5(int pStartCounter, string pName)
        {
            StartCounter = pStartCounter;
            Name = pName;
        }
    }

    class Program
    {
        static Thread t1, t2, t3, t4, t5, t6, t7;
        static bool letsFinish;
        static int idThread;
        static object objLock; // Associado ao lock() para resolver problema de concorrência.
        static Mutex objMutex; // Outra forma de resolver problema de concorrência.

        static void ThreadT1Routine()
        {
            try // Tratamento da exceção para o caso de abortagem da Thread
            {
                while (letsFinish == false)
                {
                    /*
                    lock (objLock) // Trava. Caos controlado. Uma das formas de resolver problema de concorrência.
                    {
                        idThread = 1; // Induz erro de concorrência quando não há lock
                        Console.WriteLine("Thread * 1 * -> +1s. Nº da Thread: " + idThread);
                        Thread.Sleep(1000);
                    }
                    */
                    objMutex.WaitOne(); // Trava que evita erro de concorrência.
                    idThread = 1; // Induz erro de concorrência quando não há lock/trava
                    Console.WriteLine("Thread * 1 * -> +1s. Nº da Thread: " + idThread);
                    Thread.Sleep(1000);
                    objMutex.ReleaseMutex(); // Destrava o Mutex.
                }

            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread * 1 * Exceção: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Thread * 1 * Finalizada");

            }
        }

        static void ThreadT2Routine()
        {
            try // Tratamento da exceção para o caso de abortagem da Thread
            {
                while (letsFinish == false)
                {
                    /*
                    lock (objLock) // Trava. Caos controlado. Uma das formas de resolver problema de concorrência.
                    {
                        idThread = 2; // Induz erro de concorrência quando não há lock
                        Console.WriteLine("Thread ** 2 ** -> +1s. Nº da Thread: " + idThread);
                        Thread.Sleep(1000);
                    }
                    */
                    objMutex.WaitOne(); // Trava que evita erro de concorrência.
                    idThread = 2; // Induz erro de concorrência quando não há lock/trava
                    Console.WriteLine("Thread ** 2 ** -> +1s. Nº da Thread: " + idThread);
                    Thread.Sleep(1000);
                    objMutex.ReleaseMutex(); // Destrava o Mutex.
                }

            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine("Thread ** 2 ** Exceção: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Thread ** 2 ** Finalizada");
            }
        }

        static void ThreadT3RoutineNoParameterExample()
        {
            int counter = 10;
            do
            {
                Console.WriteLine(counter);
                Thread.Sleep(250);
                counter++;
            } while (counter < 20);
        }

        static void ThreadT4RoutineExampleContainingParameter(object startCounter)
        {
            int counter = (int)startCounter;
            do
            {
                Console.WriteLine(counter);
                Thread.Sleep(250);
                counter++;
            } while (counter <= 15);
        }

        static void ThreadT5RoutineExampleContainingMoreParameters(object pInstanceOfTheClassThatContainsTheParameters)
        {
            ThreadParametersForTheExampleWithT5 theParameters = (ThreadParametersForTheExampleWithT5)pInstanceOfTheClassThatContainsTheParameters;
            int counter = theParameters.StartCounter;
            do
            {
                Console.WriteLine(counter);
                Thread.Sleep(250);
                counter++;
            } while (counter <= 30);
            Console.WriteLine($"\nContagem finalizada partindo do {theParameters.StartCounter}. Obrigado {theParameters.Name}!");
        }

        static void ThreadT6RoutineExampleContainingMoreParameters(int pStartCounter, string pName)
        {
            int counter = pStartCounter;
            do
            {
                Console.WriteLine(counter);
                Thread.Sleep(250);
                counter++;
            } while (counter <= 40);
            Console.WriteLine($"\nContagem finalizada partindo do {pStartCounter}. Obrigado {pName}!");
        }

        static void Main(string[] args)
        {
            /* Usando o Thread.Sleep para gerar um delay:
             * 
            for(int i = 1; i <= 5; i++)
            {
                Thread.Sleep(1000); // Em milisegundos
                Console.WriteLine("Passou "+ i + " segundo(s)" );
            }
            Console.WriteLine("Finalizado");
            Console.ReadKey();
            *
            */

            objLock = new object(); // Instância objeto a ser usado como trava para as Threads

            objMutex = new Mutex(); // Outra forma de evitar erro de concorrência.

            letsFinish = false; // Flag que mantem o laço das Threads t1 e t2.

            // Associa a Thread ao Método que deverá ser executado nela.
            t1 = new Thread(new ThreadStart(ThreadT1Routine));
            t2 = new Thread(new ThreadStart(ThreadT2Routine));


            /*
            - Estabelece prioridades de execução para as Threads.
            Essas prioridades serão mais relevantes/importantes,
            principalmente para o caso de tarefas que precisam ser 
            concluídas em um curto intervalo de tempo.
            - No exemplo atual, o processador ficará "chaveando" entre
            as Threads t1 e t2, e acabará executando mais frequentemente o
            Método associado à t1, pois ela tem a prioridade mais alta.
            */
            t1.Priority = ThreadPriority.Highest; // Alta prioridade, i.e, será executada com mais frequência.
            t2.Priority = ThreadPriority.BelowNormal; // Baixa prioridade, será executada com menos frequência.


            t1.Start(); // Dispara Método associado à t1
            t2.Start(); // Dispara Método associado à t2


            // O programa pode continuar enquanto os métodos de 
            // t1 e t2 rodam paralelamente.
            Console.ReadKey(); // Aguardando...

            // Vai fazer com que as Threads saiam do laço e parem de ser executadas.
            //letsFinish = true;

            /*
            - Forma alternativa de encerrar execução das Threads,
            onde elas são encerradas e não executarão mais nada.
            O programa não sairá do laço e executará o que está fora dele,
            a Thread simplesmente é abortada.Nem sempre é
            apropriado usar o Método.Abort().
            - .Abort() gera uma exceção, portanto, ao utilizá-lo,
            é recomendado implementar também um tratamento de
            exceção no método que é executado na Thread.

            t1.Abort();
            t2.Abort();
            */
            t1.Abort();
            t2.Abort();

            Thread.Sleep(1000);
            Console.WriteLine("Pressione qualquer tecla para mais exemplos com Threads\n");
            Console.ReadKey();

            // Mais exemplos:

            Console.Clear();
            Thread t3 = new Thread(new ThreadStart(ThreadT3RoutineNoParameterExample));
            Console.WriteLine("Exemplo de Thread sem parâmetro:\n");
            t3.Start();
            Console.ReadKey();

            Console.Clear();
            // No exemplo abaixo, o parâmetro do método que está sendo associado
            // à Thread, deve ser do tipo object.
            Thread t4 = new Thread(new ParameterizedThreadStart(ThreadT4RoutineExampleContainingParameter));
            Console.WriteLine("Exemplo de Thread com parâmetro:\n");
            t4.Start(5);
            Console.ReadKey();

            Console.Clear();
            // Abaixo, o método que está sendo associado à Thread t5 tem
            // mais de um parâmetro. Há pelo menos duas formas de lidarmos com
            // isso, e a primeira está aqui neste exemplo. Foi criada uma classe
            // chamada ThreadParametersForTheExampleWithT5, cujas propriedades
            // representam os parâmetros da Thread, e a instância dessa classe
            // é passada como parâmetro.
            ThreadParametersForTheExampleWithT5 ThreadParameters = new ThreadParametersForTheExampleWithT5(20, "Rafael");
            Thread t5 = new Thread(new ParameterizedThreadStart(ThreadT5RoutineExampleContainingMoreParameters));
            Console.WriteLine("Exemplo de Thread com mais de um parâmetro, onde as propriedades de uma classe são usadas como parâmetro:\n");
            t5.Start(ThreadParameters);
            Console.ReadKey();

            Console.Clear();
            // Novamente, o método que está sendo associado à Thread, agora t6,
            // tem mais de um parâmetro. Neste exemplo, vemos outra formas de lidarmos com
            // isso, que desta vez é mais direta, sem a necessidade de usar uma classe.
            // Foi criada uma classe.
            Thread t6 = new Thread(()=> ThreadT6RoutineExampleContainingMoreParameters(30, "Carla"));
            Console.WriteLine("Exemplo de Thread com mais de um parâmetro, onde os mesmos estão sendo passados de forma mais direta:\n");
            t6.Start();
            Console.ReadKey();

            Console.Clear();
            // O exemplo abaixo é muito mais direto. O corpo do método que será
            // executado na Thread, está contido na própria instanciação da mesma.
            int startCounter = 50;
            string name = "Luciane";
            Console.WriteLine("Exemplo mais direto, onde o corpo do método é declarado na instanciação da Thread t7:\n");
            Thread t7 = new Thread(() =>
            {
                int counter = startCounter;
                do
                {
                    Console.WriteLine(counter);
                    Thread.Sleep(250);
                    counter++;
                } while (counter <= 60);
                Console.WriteLine($"\nContagem da Thread t7 finalizada, partindo do {startCounter}. Obrigado {name}!");
            });
            t7.Start();
            Console.ReadKey();
            Console.Clear();
        }
    }
}
