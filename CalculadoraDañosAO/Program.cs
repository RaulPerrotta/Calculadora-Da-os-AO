using System;
using IniParser;
using IniParser.Model;
using Alba.CsConsoleFormat;
using static System.ConsoleColor;
using System.Collections.Generic;

namespace CalculadoraDañosAO
{
	class Program
	{
		static void Main()
		{

			var parser_clases = new FileIniDataParser();
			IniData clases = parser_clases.ReadFile("clases.ini");

			var parser_razas = new FileIniDataParser();
			IniData razas = parser_razas.ReadFile("razas.ini");

			var parser_armas = new FileIniDataParser();
			IniData armas = parser_armas.ReadFile("armas.ini");

			var parser_hechizos = new FileIniDataParser();
			IniData hechizos = parser_hechizos.ReadFile("hechizos.ini");

			string[] clases_array = new string[int.Parse(clases["CLASES"]["CANTIDAD"])];

			List<String> lista_armas_dagas = new List<String>();
			List<String> lista_daños_dagas_apu = new List<String>();

			List<String> lista_armas_1mano = new List<String>();
			List<String> lista_daños_1mano = new List<String>();

			List<String> lista_armas_2manos = new List<String>();
			List<String> lista_daños_2manos = new List<String>();

			List<String> lista_armas_proyectiles = new List<String>();
			List<String> lista_daños_proyectiles_comun = new List<String>();

			List<String> lista_armas_wrestling = new List<String>();
			List<String> lista_daños_wrestling = new List<String>();

			List<String> lista_hechizos = new List<String>();
			List<String> lista_daños_hechizos = new List<String>();

			List<String> lista_cps = new List<String>();

			bool correr = true;
			while (correr == true)
			{
				// Genera array de todas las clases.
				int i = 0;

				string valido = "no";
				while (valido == "no")
				{
					try
					{
						clases_array[i] = clases["CLASES"][i.ToString()];
						i += 1;
					}

					catch
					{
						valido = "si";
					}
				}

				/////inputs de nivel y fuerza			
				int nivel = 0;
				valido = "no";
				while (valido == "no")
				{
					Console.WriteLine("En que nivel se encuentran los personajes?");
					string nivel_input = Console.ReadLine();
					try
					{
						nivel = int.Parse(nivel_input);
						if (nivel > 0)
						{
							valido = "si";
						}
						else
						{
							Console.WriteLine("El nivel no puede ser negativo.");
						}
					}
					catch
					{
						Console.WriteLine("Debes ingresar un nivel valido.");
					}
				}

				valido = "no";
				int fuerza = 0;

				while (valido == "no")
				{
					Console.WriteLine("Cual es la raza de los personajes?");
					string raza = Console.ReadLine().ToUpper();
					try
					{
						fuerza = int.Parse(razas[raza]["FUE"]);
						fuerza += 18;
						valido = "si";
					}
					catch
					{
						Console.WriteLine("Debes ingresar una raza valida.");
					}
				}

				valido = "no";
				while (valido == "no")
				{
					Console.WriteLine("Los perosnajes tienen la fuerza maxeada?");
					string dopa = Console.ReadLine().ToLower();

					if (dopa == "si")
					{
						fuerza *= 2;
						if (fuerza > 40)
						{
							fuerza = 25;
							valido = "si";
						}
						else
						{
							fuerza -= 15;
							valido = "si";
						}
					}
					else if (dopa == "no")
					{
						fuerza -= 15;
						valido = "si";
					}
					else
					{
						Console.WriteLine("Debes ingresar si o no.");
					}
				}

				//input de defensa

				int def_min = 0;
				int def_max = 0;

				valido = "no";
				while (valido == "no")
				{
					Console.WriteLine("Tener en cuenta defensas?");
					string tienen_def = Console.ReadLine().ToLower();
					if (tienen_def == "si")
					{
						while (valido == "no")
						{
							Console.WriteLine("Cual es la defena minima?");
							string d_min = Console.ReadLine();
							Console.WriteLine("Cual es la defena maxima?");
							string d_max = Console.ReadLine();
							try
							{
								def_min = int.Parse(d_min);
								if (def_min < 0)
								{
									def_min = 0;
								}
								def_max = int.Parse(d_max);
								if (def_max < 0)
								{
									def_max = 0;
								}
								valido = "si";
							}
							catch
							{
								Console.WriteLine("Debes ingresar defensas validas.");
							}
						}
					}
					else if (tienen_def == "no")
					{
						valido = "si";
					}
					else
					{
						Console.WriteLine("Debes ingresar si o no.");
					}
				}

				//funcion de calculo de daños fisicos			
				string calculo_daños(int base_min, int base_max, int flecha_min, int flecha_max, int dmin_arma, int dmax_arma, int fuerza, int golpe_min, int golpe_max, double mod, bool apu, double mod_apu)
				{
					int min_min = base_min + dmin_arma + flecha_min;
					int min_max = base_max + dmax_arma + flecha_max;
					int max_max = base_max + dmax_arma;

					double dmin = 0;
					double dmax = 0;

					dmin = (min_min * 3.0 + max_max / 5.0 * fuerza + golpe_min) * mod;
					dmax = (min_max * 3.0 + max_max / 5.0 * fuerza + golpe_max) * mod;

					dmin -= def_max;
					dmax -= def_min;

					if (apu == true)
					{
						dmin *= mod_apu;
						dmax *= mod_apu;
					}

					if (dmin < 0)
					{
						dmin = 0;
					}

					if (dmax < 0)
					{
						dmax = 0;
					}

					int dmin_int = Convert.ToInt32(dmin);
					int dmax_int = Convert.ToInt32(dmax);

					string daños = dmin_int.ToString() + "-" + dmax_int.ToString();
					return daños;
				}

				//Revisa el tipo del arma, agrega el nombre del arma a una lista segun el tipo, genra una lista de cps para el arma y revisa cada clase
				//si la clase esta en la lista de cps agrega entrada vacia a la lista de daños, si no esta en la lista de cps calcula el daño del arma con los mods de 
				//la clase y agrega el daño a la lista de daños.

				bool loop = true;

				int contador_armas = 1;
				string numero_arma = "ARMA#" + contador_armas.ToString();

				bool loop_cps = true;

				int contador_cps = 1;
				string numero_cp = "CP" + contador_cps.ToString();

				void lista_daños(string tipo, List<String> lista_nombres, List<String> lista_daños, bool apuñala, string mod, int base_min, int base_max)
				{
					while (loop == true)
					{
						if (armas[numero_arma]["TIPO"] == tipo)
						{
							lista_nombres.Add(armas[numero_arma]["NOMBRE"]);

							while (loop_cps == true)
							{
								if (armas[numero_arma][numero_cp] != null)
								{
									lista_cps.Add(armas[numero_arma][numero_cp]);
									numero_cp = "CP" + (contador_cps += 1).ToString();
								}
								else
								{
									loop_cps = false;
								}
							}

							foreach (string clase in clases_array)
							{
								int flecha_min = 0;
								int flecha_max = 0;
								if (tipo == "PROYECTILES")
								{
									flecha_min = int.Parse(clases[clase]["MUNICIONMIN"]);
									flecha_max = int.Parse(clases[clase]["MUNICIONMAX"]);
								}

								if (lista_cps.Contains(clase))
								{
									lista_daños.Add("");
								}
								else
								{
									int golpe_min = 0;
									int golpe_max = 0;
									int golpe_base = int.Parse(clases[clase]["GOLPE"]);
									int golpe_35 = int.Parse(clases[clase]["GOLPE35"]);
									if (nivel <= 35)
									{
										golpe_min = golpe_base * nivel + 1;
										if (golpe_min > 99)
										{
											golpe_min = 99;
										}
										golpe_max = golpe_base * nivel + 2;
										if (golpe_max > 99)
										{
											golpe_max = 99;
										}
									}

									else if (nivel > 35)
									{
										if (golpe_base * 35 < 99)
										{
											golpe_min = golpe_base * nivel + 1;
											golpe_max = golpe_base * nivel + 2;
										}

										else
										{
											golpe_min = golpe_35 * (nivel - 35) + 99;
											golpe_max = golpe_35 * (nivel - 35) + 99;
										}
									}
									string daño = (calculo_daños(base_min, base_max, flecha_min, flecha_max, int.Parse(armas[numero_arma]["DMIN"]), int.Parse(armas[numero_arma]["DMAX"]),
										fuerza, golpe_min, golpe_max, double.Parse(clases[clase][mod]), apuñala, double.Parse(clases[clase]["MODAPU"])));
									lista_daños.Add(daño);
								}
							}

							lista_cps.Clear();
							contador_cps = 1;
							numero_cp = "CP" + contador_cps.ToString();
							contador_armas += 1;
							numero_arma = "ARMA#" + contador_armas.ToString();
							loop_cps = true;
						}

						else if (armas[numero_arma]["TIPO"] != null)
						{
							contador_armas += 1;
							numero_arma = "ARMA#" + contador_armas.ToString();
						}

						else
						{
							contador_armas = 1;
							numero_arma = "ARMA#" + contador_armas.ToString();
							loop = false;
						}
					}
					loop = true;
				}

				lista_daños("DAGAS", lista_armas_dagas, lista_daños_dagas_apu, true, "CCADMG", 0, 0);
				lista_daños("1MANO", lista_armas_1mano, lista_daños_1mano, false, "CCADMG", 0, 0);
				lista_daños("2MANOS", lista_armas_2manos, lista_daños_2manos, false, "CCADMG", 0, 0);
				lista_daños("PROYECTILES", lista_armas_proyectiles, lista_daños_proyectiles_comun, false, "PRODMG", 0, 0);
				lista_daños("PROYECTILESNM", lista_armas_proyectiles, lista_daños_proyectiles_comun, false, "PRODMG", 0, 0);
				lista_daños("WRESTLING", lista_armas_wrestling, lista_daños_wrestling, false, "CSADMG", 4, 9);

				// Calcula el daño de los hechizos.

				int index_hechizo = 1;
				string numero_hechizo = "HECHIZO#" + index_hechizo.ToString();

				contador_cps = 1;
				numero_cp = "CP" + contador_cps.ToString();

				while (hechizos[numero_hechizo]["NOMBRE"] != null)
				{
					lista_hechizos.Add(hechizos[numero_hechizo]["NOMBRE"]);
					loop_cps = true;
					while (loop_cps == true)
					{
						if (hechizos[numero_hechizo][numero_cp] != null)
						{
							lista_cps.Add(hechizos[numero_hechizo][numero_cp]);	
							numero_cp = "CP" + (contador_cps += 1).ToString();
						}
						else
						{
							loop_cps = false;
						}
					}
					foreach (string clase in clases_array)
					{
						if (lista_cps.Contains(clase))
						{
							lista_daños_hechizos.Add("");
						}
						else
						{
							double mod_magia = 1 + int.Parse(clases[clase]["MODMAGIA"]) / 100.0;
							int min = int.Parse(hechizos[numero_hechizo]["DMIN"]);
							int max = int.Parse(hechizos[numero_hechizo]["DMAX"]);
							double daño_hechizo_min = (min + (min * 3.0 * nivel) / 100.0) * mod_magia;
							double daño_hechizo_max = (max + (max * 3.0 * nivel) / 100.0) * mod_magia;
							string daños_hechizos_juntos = Convert.ToInt32(daño_hechizo_min).ToString() + "-" + Convert.ToInt32(daño_hechizo_max).ToString();
							lista_daños_hechizos.Add(daños_hechizos_juntos);
						}
					}
					numero_hechizo = "HECHIZO#" + (index_hechizo += 1).ToString();
				}

				// Crea una tabla en base a las distintas combinaciones de armas y/o hechizo, tipo de arma, daños, y clases.

				var doc = new Document(
					);

				Grid tabla = new Grid
				{
					Color = Blue,
					Columns = { },
					Children = { }

				};

				i = 0;
				while (i < clases_array.Length + 1)
				{
					tabla.Columns.Add(GridLength.Auto);
					i += 1;
				}

				void tablas(string tag, List<String> lista_armas, List<String> lista_daños)
				{
					if (lista_armas.Count > 0)
					{
						i = 0;
						int index = 0;

						tabla.Children.Add(new Cell(tag) { Color = Yellow });

						foreach (string clase in clases_array)
						{
							tabla.Children.Add(new Cell(clase) { Color = Yellow });
						}

						foreach (string arma in lista_armas)
						{
							tabla.Children.Add(new Cell(arma) { Color = Red });
							while (i < int.Parse(clases["CLASES"]["CANTIDAD"]))
							{
								tabla.Children.Add(new Cell(lista_daños[index]) { Color = Gray });
								i += 1;
								index += 1;
							}
							i = 0;
						}
					}
				}

				tablas("DAGAS", lista_armas_dagas, lista_daños_dagas_apu);
				tablas("1MANO", lista_armas_1mano, lista_daños_1mano);
				tablas("2MANOS", lista_armas_2manos, lista_daños_2manos);
				tablas("PROYECTILES", lista_armas_proyectiles, lista_daños_proyectiles_comun);
				tablas("SIN ARMAS", lista_armas_wrestling, lista_daños_wrestling);
				tablas("HECHIZOS", lista_hechizos, lista_daños_hechizos);

				doc.Children.Add(tabla);
				ConsoleRenderer.RenderDocument(doc);

				valido = "no";
				while (valido == "no")
				{
					Console.WriteLine("Correr nuevamente?");
					string correr_nuevamente = Console.ReadLine().ToLower();
					if (correr_nuevamente == "si")
					{
						valido = "si";
					}
					else if (correr_nuevamente == "no")
					{
						correr = false;
						valido = "si";
					}
					else
					{
						Console.WriteLine("Debes ingresar si o no.");
					}
				}
			}
		}
	}
}
