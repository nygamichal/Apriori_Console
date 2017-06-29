using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori_Console
{
    class Program
    {
        class Program
        {
            static string TablicaDoString<T>(T[][] tab)
            {
                string wynik = "";
                for (int i = 0; i < tab.Length; i++)
                {
                    for (int j = 0; j < tab[i].Length; j++)
                    {
                        wynik += tab[i][j].ToString() + " ";
                    }
                    wynik = wynik.Trim() + Environment.NewLine;
                }

                return wynik;
            }
            static string[][] StringToTablica(string sciezkaDoPliku)
            {
                string trescPliku = System.IO.File.ReadAllText(sciezkaDoPliku); // wczytujemy treść pliku do zmiennej
                string[] wiersze = trescPliku.Trim().Split(new char[] { '\n' }); // treść pliku dzielimy wg znaku końca linii, dzięki czemu otrzymamy każdy wiersz w oddzielnej komórce tablicy
                string[][] wczytaneDane = new string[wiersze.Length][];   // Tworzymy zmienną, która będzie przechowywała wczytane dane. Tablica będzie miała tyle wierszy ile wierszy było z wczytanego poliku

                for (int i = 0; i < wiersze.Length; i++)
                {
                    string wiersz = wiersze[i].Trim();     // przypisuję i-ty element tablicy do zmiennej wiersz
                    string[] cyfry = wiersz.Split(new char[] { ' ' });   // dzielimy wiersz po znaku spacji, dzięki czemu otrzymamy tablicę cyfry, w której każda oddzielna komórka to czyfra z wiersza
                    wczytaneDane[i] = new string[cyfry.Length];    // Do tablicy w której będą dane finalne dokładamy wiersz w postaci tablicy integerów tak długą jak długa jest tablica cyfry, czyli tyle ile było cyfr w jednym wierszu
                    for (int j = 0; j < cyfry.Length; j++)
                    {
                        string cyfra = cyfry[j].Trim(); // przypisuję j-tą cyfrę do zmiennej cyfra
                        wczytaneDane[i][j] = cyfra;
                    }
                }
                return wczytaneDane;
            }
            static void Main(string[] args)
            {
                //string sciezkaDoParagonu = @"F:/paragon-system.txt";
                string sciezkaDoParagonu = @"paragon-systemK.txt";
                string[][] paragon = StringToTablica(sciezkaDoParagonu);
                Console.WriteLine("Dane paragonu");
                Console.WriteLine("-----------------------------------");
                string wynikParagon = TablicaDoString(paragon);
                Console.Write(wynikParagon);
                /****************** Miejsce na rozwiązanie *********************************/
                //Próg częstości
                Console.WriteLine("-----------------------------------");
                Console.Write("Prosze podac próg częstości(int): ");
                int progCzestosci = Int32.Parse(Console.ReadLine());
                //int progCzestosci = 2;
                Console.WriteLine("-----------------------------------");
                //Próg jakości reguł
                Console.Write("Proszę podać próg jakości reguł(double): ");
                double wartoscDlaWarunku = Double.Parse(Console.ReadLine());
                //double wartoscDlaWarunku = 0;
                Console.WriteLine("----------------------------------");

                Console.WriteLine("Posortowany zbiór zdarzen częstych.");
                string[] zbParagSort = f1(paragon, progCzestosci);
                string wynik_f1 = string.Join("},{", zbParagSort);
                Console.WriteLine("F1={{" + wynik_f1 + "}}");
                Console.WriteLine("-----------------------------------");

                Console.WriteLine("Kombinacje bez powtórzeń elementów zbioru F1, tworzą zbiór kandydatów:");
                Dictionary<int, List<string>> zbC2 = c2(zbParagSort);
                Console.Write("C2={");
                foreach (KeyValuePair<int, List<string>> kvp in zbC2)
                {
                    Console.Write(" {");
                    string[] znaki = kvp.Value.ToArray();
                    string wynik_ostateczny = string.Join(",", znaki);
                    Console.Write("{0}", wynik_ostateczny);
                    Console.Write("} ");
                    //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, wynik_ostateczny);
                }
                Console.Write("}\n--------------------------");

                Console.WriteLine("\nKandydaci posiadajacy czestosc przynajmniej dwa:");
                Dictionary<int, List<string>> zbF2 = f2(paragon, zbParagSort, zbC2, progCzestosci);
                Console.Write("F2={");
                foreach (KeyValuePair<int, List<string>> kvp in zbF2)
                {
                    Console.Write(" {");
                    string[] znaki = kvp.Value.ToArray();
                    string wynik_ostateczny = string.Join(",", znaki);
                    Console.Write("{0}", wynik_ostateczny);
                    Console.Write("} ");
                }
                Console.Write("}\n--------------------------");
                Dictionary<int, List<string>> regulyAsocjacyjneF = new Dictionary<int, List<string>>();
                regulyAsocjacyjneF.Add(2, regulyAsocjacyjne(wartoscDlaWarunku, paragon, zbF2, 2));


                Dictionary<int, List<string>> zbF = zbF2;
                int iloscElementowWzbF = 0;
                int wartoscK = 3;
                do
                {
                    Console.WriteLine("\nC" + wartoscK + ":");
                    Dictionary<int, List<string>> zbC = ck(zbF, wartoscK);
                    Console.Write("C" + wartoscK + "={");
                    foreach (KeyValuePair<int, List<string>> kvp in zbC)
                    {
                        Console.Write(" {");
                        string[] znaki = kvp.Value.ToArray();
                        string wynik_ostateczny = string.Join(",", znaki);
                        Console.Write("{0}", wynik_ostateczny);
                        Console.Write("} ");
                    }
                    Console.Write("}\n--------------------------");

                    Console.WriteLine("\nF" + wartoscK + ":");
                    zbF = fk(zbC, zbF, paragon, wartoscK);
                    iloscElementowWzbF = zbF.Count();
                    Console.Write("F" + wartoscK + "={");
                    foreach (KeyValuePair<int, List<string>> kvp in zbF)
                    {
                        Console.Write(" {");
                        string[] znaki = kvp.Value.ToArray();
                        string wynik_ostateczny = string.Join(",", znaki);
                        Console.Write("{0}", wynik_ostateczny);
                        Console.Write("} ");
                    }
                    Console.Write("}\n--------------------------");
                    regulyAsocjacyjneF.Add(wartoscK, regulyAsocjacyjne(wartoscDlaWarunku, paragon, zbF, wartoscK));
                    wartoscK += 1;
                } while (iloscElementowWzbF > 1);


                Console.Write("\n~~~~~~~~~~Regułu Asocjacyjne~~~~~~~~~~");

                //Wyszukanie kluczy, które odpowiadają ilościom reguł do wypisania
                List<int> kluczeRegul = new List<int>();
                foreach (KeyValuePair<int, List<string>> kvp in regulyAsocjacyjneF)
                {
                    kluczeRegul.Add(kvp.Key);
                }

                //wypisywanie na ekranie(w konsoli) treści reguł dla konkretnego zbioru Fk
                for (int iloscRegul = kluczeRegul.Count() - 1; iloscRegul >= 0; iloscRegul--)//dekrementacja
                {
                    Console.Write("\nZe zbioru F{0} mamy:", kluczeRegul[iloscRegul]);
                    for (int elementReguly = 0; elementReguly < regulyAsocjacyjneF[kluczeRegul[iloscRegul]].Count(); elementReguly++)
                    {
                        //wypisywanie dla konkretnego zbioru Fk Key=[kluczeRegul[iloscRegul]]   reguł znajdujacych sie w Value
                        Console.Write(regulyAsocjacyjneF[kluczeRegul[iloscRegul]].ElementAt(elementReguly));
                    }
                    Console.WriteLine();
                }
                /****************** Koniec miejsca na rozwiązanie ********************************/
                Console.ReadKey();
            }
            static string[] f1(string[][] paragon, int progCzestosci)//zrób elementów częstych
            {
                Dictionary<string, int> zbElementow = new Dictionary<string, int>();
                List<string> zbiorZdarzenCzestych = new List<string>();
                for (int nrParagonu = 0; nrParagonu < paragon.Length; nrParagonu++)//wiersz
                {
                    for (int element = 0; element < paragon[nrParagonu].Length; element++)//atrybut
                    {
                        string sprawdzanyElementParagonu = paragon[nrParagonu][element];
                        if (zbElementow.ContainsKey(sprawdzanyElementParagonu))//jeżeli dany element znajduje się w kluczu zbElementow to dodajemy do wartosci(value) jego wystąpień liczbę 1
                            zbElementow[sprawdzanyElementParagonu] += 1;
                        else//jeżeli nie występuje to go dodajemy z ilością wystąpień 1
                            zbElementow.Add(sprawdzanyElementParagonu, 1);
                        if (zbElementow.ContainsKey(sprawdzanyElementParagonu) && zbElementow[sprawdzanyElementParagonu] >= progCzestosci && !zbiorZdarzenCzestych.Contains(sprawdzanyElementParagonu))//jeżeli Dictionary zawiera ten element jako Key ORAZ jego Value jest większe bądź równe "progCzestosci" ORAZ List zbiorZdarzenCzestych NIE zawiera tego elementu w List
                            zbiorZdarzenCzestych.Add(sprawdzanyElementParagonu);
                    }
                }
                zbiorZdarzenCzestych.Sort();//Sortowanie
                return zbiorZdarzenCzestych.ToArray();
            }
            static Dictionary<int, List<string>> c2(string[] zbParagSort)//kombinacje bez powtórzeń dla F1
            {
                List<string> listaPodzbioru;
                Dictionary<int, List<string>> zbiorKomBezPowt = new Dictionary<int, List<string>>();
                int k = 0;
                for (int i = 1; i < zbParagSort.Length; i++)
                {
                    for (int j = i; j < zbParagSort.Length; j++)
                    {
                        listaPodzbioru = new List<string>();//czyścimy listę oraz tworzymy dla niej nowe miejsce, adresowanie, coś takiego
                        listaPodzbioru.Add(zbParagSort[i - 1]);
                        listaPodzbioru.Add(zbParagSort[j]);
                        zbiorKomBezPowt.Add(k++, listaPodzbioru);
                        //listaPodzbioru.Clear(); -- tego nie robić, gdyż odwołuje się do tego samego adresu co pierwsza zalalokwoana zmienna tej nazwy(jakoś tak)
                    }
                }
                return zbiorKomBezPowt;
            }
            static Dictionary<int, List<string>> f2(string[][] paragon, string[] zbParagSort, Dictionary<int, List<string>> zbC2, int progCzestosci)
            {
                Dictionary<int, List<string>> zbiorKomBezPowt = new Dictionary<int, List<string>>();
                List<int> keyKandydata = new List<int>();
                int kandydatC = 0;//gdyż ilość elementów w podzbiorach C2 wynosi 2 dlatego kandydatC = 0 zaś kandydatC+1 = 1 i osiągamy w ten sposóbb wszystkie leementy z C2
                int wystapieniaKandydata1 = 0;
                int wystapieniaKandydata2 = 0;
                int wystapienNaParagonie = 0;
                for (int obiektowC2 = 0; obiektowC2 < zbC2.Count(); obiektowC2++)
                {
                    //zmiana podzbioru C2, więc zerujemy wystąpienia na paragonach
                    wystapienNaParagonie = 0;
                    for (int nrParagonu = 0; nrParagonu < paragon.Length; nrParagonu++)//wiersz
                    {
                        //zmiana paragonu, więc zerujemy wystąpienia Kandydatów w tymże paragonie
                        wystapieniaKandydata1 = 0;
                        wystapieniaKandydata2 = 0;
                        for (int element = 0; element < paragon[nrParagonu].Length; element++)//kolumna
                        {
                            if (zbC2[obiektowC2].ElementAt(kandydatC) == paragon[nrParagonu][element])
                            {
                                wystapieniaKandydata1++;
                            }
                            if (zbC2[obiektowC2].ElementAt(kandydatC + 1) == paragon[nrParagonu][element])
                            {
                                wystapieniaKandydata2++;
                            }
                            if ((wystapieniaKandydata1 + wystapieniaKandydata2) == progCzestosci)
                            //jeżeli suma wystąpień tych dwóch kandydatów wynosi 2 to wartość wystapienNaParagonie ulega inkrementacji, zaś ilosc wystapien tychże kandydatów zeruje się
                            {
                                wystapienNaParagonie++;
                                wystapieniaKandydata1 = 0;
                                wystapieniaKandydata2 = 0;
                            }
                            if (wystapienNaParagonie >= progCzestosci && !keyKandydata.Contains(obiektowC2))
                            {
                                keyKandydata.Add(obiektowC2);
                            }
                        }
                    }
                }
                //numery kluczy zaczynały się od 0, a nie od jakiejś innej liczby, która trudniałaby operacje w przyszłości(należało by sprawdzać czy dany klucz istnieje w słowniku i dopiero pracować na nim)
                for (int i = 0; i < keyKandydata.Count(); i++)
                {
                    zbiorKomBezPowt.Add(i, new List<string>());
                    zbiorKomBezPowt[i] = zbC2[keyKandydata[i]];
                }
                return zbiorKomBezPowt;
            }
            static Dictionary<int, List<string>> ck(Dictionary<int, List<string>> zbF, int wartoscK)
            {
                Dictionary<int, List<string>> zbiorCk = new Dictionary<int, List<string>>();
                List<string> listaElementowZbioruCk = new List<string>();
                int keyZbioruCk = 0;
                int iloscWystopienWspolnychElementow = 0;
                int iloscWspolnychPozycji = wartoscK - 2;//k-2

                for (int nrParagonu = 0; nrParagonu < zbF.Count() - 1; nrParagonu++)//ilosc paragonów porównywanych do następnych(bez ostatniego, gdyż po nim nie występuje żaden następny
                {
                    for (int nrParagonu2 = nrParagonu + 1; nrParagonu2 < zbF.Count(); nrParagonu2++)
                    {
                        iloscWystopienWspolnychElementow = 0;//zmiana paragonu, który jest porównywany do następnych paragonów, więc zerujemy ilość jego wystąpień
                        listaElementowZbioruCk = new List<string>();//List elementów zbioru Ck, czyli tych posiadających k-2 wspólnych pierwszych pozycji
                        for (int elementBranyDoPodzbioru = 0; elementBranyDoPodzbioru < iloscWspolnychPozycji; elementBranyDoPodzbioru++)
                        {
                            if (zbF[nrParagonu].ElementAt(elementBranyDoPodzbioru) == zbF[nrParagonu2].ElementAt(elementBranyDoPodzbioru))
                            {
                                listaElementowZbioruCk.Add(zbF[nrParagonu].ElementAt(elementBranyDoPodzbioru));//dodaje do listy wspolny element paragonu1 i paragonu2 znajdujący się w List na miescju nr elementBranyDoPodzbioru
                                iloscWystopienWspolnychElementow++;//ikrementacja wystąpienia tych samych elementów
                            }
                            if (iloscWystopienWspolnychElementow == iloscWspolnychPozycji)//List listaElementowZbioruCk zawiera tyle elementów ile miało być wspólnych decyzji to ostatnie elementy tych podzbiorów dostają dodane do listy
                            {
                                listaElementowZbioruCk.Add(zbF[nrParagonu].ElementAt(elementBranyDoPodzbioru + 1));
                                listaElementowZbioruCk.Add(zbF[nrParagonu2].ElementAt(elementBranyDoPodzbioru + 1));
                                zbiorCk.Add(keyZbioruCk++, listaElementowZbioruCk);//klucze zbioru Ck zaczynają się od 0 i kończą na ilości pozbiorów
                                listaElementowZbioruCk = new List<string>();
                                iloscWystopienWspolnychElementow = 0;
                            }
                        }
                    }
                }
                return zbiorCk;
            }
            static Dictionary<int, List<string>> fk(Dictionary<int, List<string>> zbC, Dictionary<int, List<string>> zbF, string[][] paragon, int wartoscK)
            {
                int rozmiarPodzbiorow = wartoscK - 1;

                Dictionary<int, List<string>> zbiorPodzbiorowZbC = new Dictionary<int, List<string>>();
                int kluczZbioruPodzbiorowCk = 0;

                Dictionary<int, List<string>> ck_podzbioryCzeste = new Dictionary<int, List<string>>();
                int kluczCk_podzbioryCzeste = 0;
                List<string> podzbioryCk;

                Dictionary<int, List<string>> ck_podzbiorySpr = new Dictionary<int, List<string>>();
                Dictionary<int, List<string>> fk = new Dictionary<int, List<string>>();

                List<int> nrKluczyDoZbC = new List<int>();

                //Numery kluczy słownika zbF
                List<int> kluczeZbF = new List<int>();
                foreach (KeyValuePair<int, List<string>> kvp in zbF)
                {
                    kluczeZbF.Add(kvp.Key);
                }

                //Tworzone są podzbiory ze zbiorów Ck o długości k-1(kombinacje bez powtorzen), by sprawdzić ich częstość, jeżeli którykolwiek z podzbiorów zbioru Ck nie jest częsty to konkretny zbiór Ck jest usuwany ze zbiorów Ck
                for (int kluczZbC = 0; kluczZbC < zbC.Count(); kluczZbC++)//ilosc kluczy w zbC
                {
                    kluczZbioruPodzbiorowCk = 0;//zerujemy kluczZbioruPodzbiorówCk dla Dictionary zbiorPodzbiorowZbC
                    for (int elementZbC = 0; elementZbC < zbC[kluczZbC].Count(); elementZbC++)
                    {
                        podzbioryCk = new List<string>();
                        //poniższy for wykona się tyle razy ile wynosi rozmiar podzbiorów dla konkretnego założenia
                        for (int iloscPowtorzenWykonaniaDzialania = 0; iloscPowtorzenWykonaniaDzialania < rozmiarPodzbiorow; iloscPowtorzenWykonaniaDzialania++)
                        {
                            podzbioryCk.Add(zbC[kluczZbC].ElementAt(elementZbC));
                            if (!(podzbioryCk.Count() == rozmiarPodzbiorow) && elementZbC < zbC[kluczZbC].Count())
                            {
                                elementZbC++;//inkrementacja w celu dodania przy kolejnej iteracji kolejnego elementuZbC
                            }
                            if (elementZbC == zbC[kluczZbC].Count())//ostatni element zbC[kluczZbC] to zmieniamy go na zerowy element zbioru
                            {
                                elementZbC = 0;
                            }

                            //gdy List podzbiorCk osiągnie rozmiar o wartosci rozmiarPodzbiorow, Lista ta zostaje posortowana alfabetycznie od A do Z i dodana do Dictionary zbiorPodzbiorowZbC Key=kluczZbioruPodzbioroCk(po dodaniu wystepuje jego inkrementacja) Value=podzbiorCk
                            //dodatkowo, jeżeli elementZbC jest inny niż 0 to następuje jego dekrementacja, aby zacząć następną iterację od ostaniego elementu podzbioru poprzedniej iteracji
                            if (podzbioryCk.Count() == rozmiarPodzbiorow)
                            {
                                if (!(elementZbC == 0))
                                {
                                    elementZbC--;
                                }
                                podzbioryCk.Sort();
                                zbiorPodzbiorowZbC.Add(kluczZbioruPodzbiorowCk++, podzbioryCk);
                                podzbioryCk = new List<string>();//wyczyszczenie List
                            }
                        }
                        if (zbiorPodzbiorowZbC.Count() == wartoscK)
                        //jeżeli ilość kluczy zbiorPodzbiorówZbC wynosi wartośćK to kończymy iterację, gdyż wszystkie możliwości zostały osiągnięte, skąd to wiadomo? gdyż z elementów zbioru {Kabaczki,ogórki,pomidory} jesteśmy zuyskać tylko 3 podzbiory(akurat wartośćK) są to:
                        //{kabaczki,ogorki},{kabaczki,pomidory},{ogorki,pomidory}  
                        {
                            elementZbC = zbC[kluczZbC].Count();//elementZbC wypisujemy ostatnią wartość iteracji w celu zakończenia wykonywania 
                                                               //break;
                        }
                    }

                    kluczCk_podzbioryCzeste = 0;

                    //sprawdzamy częstość podzbiorów zbioruCk[kluczZbC] czyli zbiorów zawartych w Dictionary zbiorPodbiorowZbC
                    for (int kluczZbPodzbiorow = 0; kluczZbPodzbiorow < zbiorPodzbiorowZbC.Count(); kluczZbPodzbiorow++)//ilosc zbiorow w zbiorPodzbiorowZbC
                    {
                        for (int kluczFk = 0; kluczFk < zbF.Count(); kluczFk++)
                        {
                            int ilsocTychSamychElementow = 0;
                            for (int elementPodzbioru = 0; elementPodzbioru < rozmiarPodzbiorow; elementPodzbioru++)
                            {
                                if (zbF[kluczFk].ElementAt(elementPodzbioru) == zbiorPodzbiorowZbC[kluczZbPodzbiorow].ElementAt(elementPodzbioru))
                                {
                                    ilsocTychSamychElementow++;
                                }
                                if (ilsocTychSamychElementow == rozmiarPodzbiorow)
                                {
                                    ck_podzbioryCzeste.Add(kluczCk_podzbioryCzeste++, zbiorPodzbiorowZbC[kluczZbPodzbiorow]);
                                }

                            }
                        }
                    }

                    //pomoc by uzyskac klucze zbiorów C3, które zostąły po wyrzuceniu podzbiorów nie spełniających częstości 
                    if (zbiorPodzbiorowZbC.Count() == ck_podzbioryCzeste.Count())
                    {
                        for (int kluczCk = 0; kluczCk < ck_podzbioryCzeste.Count(); kluczCk++)
                        {
                            podzbioryCk = new List<string>();
                            for (int elementPodzbioru = 0; elementPodzbioru < rozmiarPodzbiorow; elementPodzbioru++)
                            {
                                podzbioryCk.Add(ck_podzbioryCzeste[kluczCk].ElementAt(elementPodzbioru));
                            }
                            ck_podzbiorySpr.Add(kluczCk, podzbioryCk);//podzbiory
                        }
                        nrKluczyDoZbC.Add(kluczZbC);
                    }
                    zbiorPodzbiorowZbC = new Dictionary<int, List<string>>();
                    ck_podzbioryCzeste = new Dictionary<int, List<string>>();
                    ck_podzbiorySpr = new Dictionary<int, List<string>>();
                }

                //sprawdzamy częstość zbiorów Ck
                int wystapienWtymWierszu = 0, wystapienNaParagonie = 0;
                for (int iloscNrKluczyDoCk = 0; iloscNrKluczyDoCk < nrKluczyDoZbC.Count(); iloscNrKluczyDoCk++)
                {
                    wystapienNaParagonie = 0;
                    for (int wiersz = 0; wiersz < paragon.Length; wiersz++)
                    {
                        wystapienWtymWierszu = 0;
                        for (int kolumna = 0; kolumna < paragon[wiersz].Length; kolumna++)
                        {
                            for (int elementListy = 0; elementListy < zbC[nrKluczyDoZbC[iloscNrKluczyDoCk]].Count(); elementListy++)
                            {
                                if (paragon[wiersz][kolumna] == zbC[nrKluczyDoZbC[iloscNrKluczyDoCk]].ElementAt(elementListy))
                                {
                                    wystapienWtymWierszu++;
                                }
                            }
                        }
                        if (wystapienWtymWierszu == zbC[nrKluczyDoZbC[iloscNrKluczyDoCk]].Count())
                        {
                            wystapienNaParagonie += 1;
                        }
                    }
                    if (wystapienNaParagonie >= 2)
                    {
                        fk.Add(iloscNrKluczyDoCk, zbC[nrKluczyDoZbC[iloscNrKluczyDoCk]]);
                    }
                }
                return fk;
            }
            static List<string> regulyAsocjacyjne(double wartoscDlaWarunku, string[][] paragon, Dictionary<int, List<string>> zbF, int wartoscK)
            {
                List<string> trescReguly = new List<string>();
                double wsp = 0, ufn = 0, wynik = 0;
                double mianownikWsp = paragon.Length;
                int rozmiarPodzbiorow = wartoscK - 1;

                Dictionary<int, List<string>> regulyAsocjacyjne;
                List<string> podzbioryFk;
                int kluczZbioruPodzbiorowFk = 0;


                //Numery kluczy słownika zbF
                List<int> kluczeZbF = new List<int>();
                foreach (KeyValuePair<int, List<string>> kvp in zbF)
                {
                    kluczeZbF.Add(kvp.Key);
                }

                for (int kluczZbF = 0; kluczZbF < zbF.Count(); kluczZbF++)//ilosc kluczy
                {
                    //qpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpaqpa
                    //nie potrzebne sprawdzanie czy klucz wystepuje w Dictionary, gdyż we wcześniejszej metodzie, ustalamy iż ZbF zaczynały się klucze od 0 i kończyły na ilości wierszy.
                    //if (zbF.ContainsKey(kluczeZbF[kluczZbF]))
                    //{
                    kluczZbioruPodzbiorowFk = 0;//zerujemy kluczZbioruPodzbiorówFk dla Dictionary zbiorPodzbiorowZbF
                    regulyAsocjacyjne = new Dictionary<int, List<string>>();

                    //kombinacje bez powtorzen
                    for (int elementZbF = 0; elementZbF < zbF[kluczZbF].Count(); elementZbF++)//ile paragonów/wierszy
                    {
                        podzbioryFk = new List<string>();
                        //poniższy for wykona się tyle razy ile wynosi rozmiar podzbiorów dla konkretnego założenia
                        for (int elementPodzbioru = 0; elementPodzbioru < rozmiarPodzbiorow; elementPodzbioru++)
                        {
                            podzbioryFk.Add(zbF[kluczZbF].ElementAt(elementZbF));
                            if (!(podzbioryFk.Count() == rozmiarPodzbiorow + 1) && elementZbF < zbF[kluczZbF].Count())
                            {
                                elementZbF++;//inkrementacja w celu dodania przy kolejnej iteracji kolejnego elementuZbF
                            }
                            if (elementZbF == zbF[kluczZbF].Count())//ostatni element zbC[kluczZbC] to zmieniamy go na zerowy element zbioru
                            {
                                elementZbF = 0;
                            }


                            //gdy List podzbioryFk osiągnie rozmiar o wartosci rozmiarPodzbiorow, Lista ta zostaje posortowana alfabetycznie od A do Z i dodana do Dictionary regulyAsocjacyjne Key=kluczZbioruPodzbioroCk(po dodaniu wystepuje jego inkrementacja) Value=podzbioryFk
                            //dodatkowo, jeżeli elementZbF jest inny niż 0 to następuje jego dekrementacja, aby zacząć następną iterację od ostaniego elementu podzbioru poprzedniej iteracji
                            if (podzbioryFk.Count() == rozmiarPodzbiorow)
                            {
                                if (!(elementZbF == 0))
                                {
                                    elementZbF--;
                                }
                                podzbioryFk.Sort();

                                //dodatkowy for po to by dodać do podzbioruFk element który nie należy do zbioru kombinacji bez powtórzeń czyli nasz nastepnik
                                for (int elementZbF_2 = 0; elementZbF_2 < zbF[kluczeZbF[kluczZbF]].Count(); elementZbF_2++)
                                {
                                    if (!podzbioryFk.Contains(zbF[kluczeZbF[kluczZbF]].ElementAt(elementZbF_2)))
                                    {
                                        podzbioryFk.Add(zbF[kluczeZbF[kluczZbF]].ElementAt(elementZbF_2));
                                    }
                                }
                                //podzbiory.Sort();
                                regulyAsocjacyjne.Add(kluczZbioruPodzbiorowFk++, podzbioryFk);
                                podzbioryFk = new List<string>();//wyczyszczenie List
                            }
                        }
                        if (regulyAsocjacyjne.Count() == wartoscK)//jeżeli ilość kluczy zbiorPodzbiorówZbF wynosi wartośćK to kończymy iterację, gdyż wszystkie możliwości zostały osiągnięte, skąd to wiadomo? gdyż z elementów zbioru {Kabaczki,ogórki,pomidory} jesteśmy zuyskać tylko 3 podzbiory(akurat wartośćK) są to:
                                                                  //{kabaczki,ogorki},{kabaczki,pomidory},{ogorki,pomidory}  
                        {
                            elementZbF = zbF[kluczZbF].Count();//elementZbF przypisujemy ostatnią wartość iteracji w celu zakończenia wykonywania 
                            //break;
                        }
                    }

                    for (int kluczRegulyAsocjacyjnej = 0; kluczRegulyAsocjacyjnej < regulyAsocjacyjne.Count(); kluczRegulyAsocjacyjnej++)
                    {
                        if (regulyAsocjacyjne.ContainsKey(kluczRegulyAsocjacyjnej))
                        {
                            double mianownikUfn = 0, licznik = 0, wystapieniaKandydata2 = 0, wystapieniaKandydata1 = 0;
                            //regulyAsocjacyjne[kluczRegulyAsocjacyjnej].Count();
                            for (int wiersz = 0; wiersz < paragon.Length; wiersz++)
                            {
                                wystapieniaKandydata1 = 0;
                                wystapieniaKandydata2 = 0;
                                for (int kolumna = 0; kolumna < paragon[wiersz].Length; kolumna++)
                                {
                                    for (int kandydat = 0; kandydat < regulyAsocjacyjne[kluczRegulyAsocjacyjnej].Count(); kandydat++)
                                    {
                                        if (regulyAsocjacyjne[kluczRegulyAsocjacyjnej].ElementAt(kandydat) == paragon[wiersz][kolumna])
                                        {
                                            wystapieniaKandydata1++;
                                            if (wystapieniaKandydata1 >= regulyAsocjacyjne[kluczRegulyAsocjacyjnej].Count())
                                            {
                                                licznik += 1;
                                            }
                                        }
                                    }
                                    for (int kandydat = 0; kandydat < regulyAsocjacyjne[kluczRegulyAsocjacyjnej].Count() - 1; kandydat++)
                                    {
                                        if (regulyAsocjacyjne[kluczRegulyAsocjacyjnej].ElementAt(kandydat) == paragon[wiersz][kolumna])
                                        {
                                            wystapieniaKandydata2++;
                                            if (wystapieniaKandydata2 >= regulyAsocjacyjne[kluczRegulyAsocjacyjnej].Count() - 1)
                                            {
                                                mianownikUfn += 1;
                                            }
                                        }
                                    }
                                }
                            }
                            wsp = licznik / mianownikWsp;
                            ufn = licznik / mianownikUfn;
                            wynik = wsp * ufn;

                            if (wynik >= wartoscDlaWarunku)
                            {
                                trescReguly.Add("\n");
                                for (int ileRazyWypisac = 0; ileRazyWypisac < regulyAsocjacyjne[kluczRegulyAsocjacyjnej].Count(); ileRazyWypisac++)
                                {
                                    if (ileRazyWypisac < zbF[kluczeZbF[kluczZbF]].Count() - 1)
                                    {
                                        trescReguly.Add(regulyAsocjacyjne[kluczRegulyAsocjacyjnej].ElementAt(ileRazyWypisac) + " ");
                                    }
                                    if (ileRazyWypisac == zbF[kluczeZbF[kluczZbF]].Count() - 1)
                                    {
                                        trescReguly.Add("=> " + regulyAsocjacyjne[kluczRegulyAsocjacyjnej].ElementAt(ileRazyWypisac));
                                    }
                                }
                                trescReguly.Add(", wsp=" + wsp + ", ufn=" + ufn + ", wsp*ufn =" + wynik);
                            }
                        }
                    }
                    //}
                }
                return trescReguly;
            }
        }
    }
}
