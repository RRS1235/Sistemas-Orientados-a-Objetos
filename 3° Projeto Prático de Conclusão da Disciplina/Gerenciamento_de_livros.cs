using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gerenciamento_de_livros
{
    internal class Program
    {
        // Instância da classe Biblioteca para gerenciar os livros
        private Biblioteca biblioteca = new Biblioteca();

        public static void Main(string[] args)
        {
            // Cria uma instância de Program para acessar os métodos não estáticos
            Program sistema = new Program();
            sistema.ExecutarSistema();
        }

        public void ExecutarSistema()
        {
            Console.WriteLine("Bem-vindo ao Sistema de Gerenciamento de Livros da Biblioteca!");

            while (true)
            {
                ExibirMenuPrincipal();
                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CadastrarLivroUI();
                        break;
                    case "2":
                        biblioteca.ListarLivros();
                        break;
                    case "3":
                        BuscarLivroUI();
                        break;
                    case "4":
                        AtualizarLivroUI();
                        break;
                    case "5":
                        RemoverLivroUI();
                        break;
                    case "0":
                        Console.WriteLine("Saindo do sistema. Até mais!");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Por favor, tente novamente.");
                        break;
                }
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
                Console.Clear(); // Limpa a tela para o próximo menu
            }
        }

        private void ExibirMenuPrincipal()
        {
            Console.WriteLine("\n--- Menu Principal ---");
            Console.WriteLine("1. Cadastrar Livro");
            Console.WriteLine("2. Listar Livros");
            Console.WriteLine("3. Buscar Livros");
            Console.WriteLine("4. Atualizar Informacoes do Livro");
            Console.WriteLine("5. Remover Livro");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha uma opcao: ");
        }

        private void CadastrarLivroUI()
        {
            Console.WriteLine("\n--- Cadastrar Novo Livro ---");
            Console.Write("Titulo: ");
            string titulo = Console.ReadLine();
            Console.Write("Autor: ");
            string autor = Console.ReadLine();
            Console.Write("ISBN (identificador unico): ");
            string isbn = Console.ReadLine();

            biblioteca.CadastrarLivro(titulo, autor, isbn);
        }

        private void BuscarLivroUI()
        {
            Console.WriteLine("\n--- Buscar Livro ---");
            Console.WriteLine("Buscar por (1) Titulo ou (2) Autor?");
            Console.Write("Escolha uma opcao: ");
            string opcaoBusca = Console.ReadLine();

            if (opcaoBusca == "1")
            {
                Console.Write("Digite o Titulo para buscar: ");
                string titulo = Console.ReadLine();
                var livrosEncontrados = biblioteca.BuscarLivrosPorTitulo(titulo);
                if (livrosEncontrados.Any())
                {
                    Console.WriteLine("\n--- Livros Encontrados por Titulo ---");
                    foreach (var livro in livrosEncontrados)
                    {
                        Console.WriteLine(livro);
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum livro encontrado com este titulo.");
                }
            }
            else if (opcaoBusca == "2")
            {
                Console.Write("Digite o Autor para buscar: ");
                string autor = Console.ReadLine();
                var livrosEncontrados = biblioteca.BuscarLivrosPorAutor(autor);
                if (livrosEncontrados.Any())
                {
                    Console.WriteLine("\n--- Livros Encontrados por Autor ---");
                    foreach (var livro in livrosEncontrados)
                    {
                        Console.WriteLine(livro);
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum livro encontrado com este autor.");
                }
            }
            else
            {
                Console.WriteLine("Opcao de busca invalida.");
            }
        }

        private void AtualizarLivroUI()
        {
            Console.WriteLine("\n--- Atualizar Informacoes do Livro ---");
            Console.Write("Digite o ISBN do livro que deseja atualizar: ");
            string isbn = Console.ReadLine();

            var livroParaAtualizar = biblioteca.BuscarLivroPorISBN(isbn);
            if (livroParaAtualizar == null)
            {
                Console.WriteLine("Livro com o ISBN informado nao encontrado.");
                return;
            }

            Console.WriteLine($"Livro atual: {livroParaAtualizar}");
            Console.WriteLine("Deixe em branco para manter o valor atual.");

            Console.Write($"Novo Titulo ({livroParaAtualizar.Titulo}): ");
            string novoTitulo = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novoTitulo))
            {
                livroParaAtualizar.Titulo = novoTitulo;
            }

            Console.Write($"Novo Autor ({livroParaAtualizar.Autor}): ");
            string novoAutor = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(novoAutor))
            {
                livroParaAtualizar.Autor = novoAutor;
            }

            // Não permitimos atualizar o ISBN diretamente por aqui, pois ele é a chave de identificação.
            // Se precisar de atualização de ISBN, seria uma operação mais complexa (remover e adicionar).
            // Por enquanto, ele é a chave imutável para a busca e remoção.

            biblioteca.AtualizarLivro(livroParaAtualizar); // Apenas chama para exibir mensagem de sucesso
        }

        private void RemoverLivroUI()
        {
            Console.WriteLine("\n--- Remover Livro ---");
            Console.Write("Digite o ISBN do livro que deseja remover: ");
            string isbn = Console.ReadLine();

            biblioteca.RemoverLivro(isbn);
        }


        // --- Classes de Modelo (Entidades) ---
        public class Livro
        {
            public string Titulo { get; set; }
            public string Autor { get; set; }
            public string ISBN { get; set; } // Identificador Único

            public Livro(string titulo, string autor, string isbn)
            {
                Titulo = titulo;
                Autor = autor;
                ISBN = isbn;
            }

            public override string ToString()
            {
                return $"[ISBN: {ISBN}] Titulo: {Titulo} - Autor: {Autor}";
            }
        }

        // --- Classe de Gerenciamento ---
        public class Biblioteca
        {
            // Lista para iterar sobre todos os livros
            private List<Livro> livrosList;
            // Dicionário para busca rápida por ISBN e para garantir unicidade
            private Dictionary<string, Livro> livrosDictionary;

            public Biblioteca()
            {
                livrosList = new List<Livro>();
                livrosDictionary = new Dictionary<string, Livro>(StringComparer.OrdinalIgnoreCase); // Ignora maiúsculas/minúsculas no ISBN
            }

            // 1. Cadastrar livros
            public void CadastrarLivro(string titulo, string autor, string isbn)
            {
                if (livrosDictionary.ContainsKey(isbn))
                {
                    Console.WriteLine($"Erro: Já existe um livro com o ISBN '{isbn}'.");
                    return;
                }

                Livro novoLivro = new Livro(titulo, autor, isbn);
                livrosList.Add(novoLivro);
                livrosDictionary.Add(isbn, novoLivro);
                Console.WriteLine($"Livro '{titulo}' cadastrado com sucesso!");
            }

            // 2. Listar livros
            public void ListarLivros()
            {
                if (livrosList.Count == 0)
                {
                    Console.WriteLine("Nenhum livro cadastrado na biblioteca.");
                    return;
                }
                Console.WriteLine("\n--- Livros Cadastrados ---");
                foreach (var livro in livrosList)
                {
                    Console.WriteLine(livro);
                }
            }

            // 3. Buscar livros por título ou autor
            public List<Livro> BuscarLivrosPorTitulo(string titulo)
            {
                // Usamos LINQ para filtrar a lista por título (case-insensitive)
                return livrosList.Where(l => l.Titulo.IndexOf(titulo, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            public List<Livro> BuscarLivrosPorAutor(string autor)
            {
                // Usamos LINQ para filtrar a lista por autor (case-insensitive)
                return livrosList.Where(l => l.Autor.IndexOf(autor, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            // Método auxiliar para buscar por ISBN (útil para atualizar/remover)
            public Livro BuscarLivroPorISBN(string isbn)
            {
                livrosDictionary.TryGetValue(isbn, out Livro livro);
                return livro;
            }


            // 4. Atualizar informações do livro
            public void AtualizarLivro(Livro livroAtualizado)
            {
                // Como a atualização de título/autor já é feita no objeto retornado por BuscarLivroPorISBN,
                // e os objetos na lista/dicionário são referências, a alteração já se reflete.
                // Aqui, apenas confirmamos a operação.
                Console.WriteLine($"Livro com ISBN '{livroAtualizado.ISBN}' atualizado com sucesso!");
            }

            // 5. Remover livros
            public void RemoverLivro(string isbn)
            {
                if (livrosDictionary.ContainsKey(isbn))
                {
                    Livro livroParaRemover = livrosDictionary[isbn];
                    livrosList.Remove(livroParaRemover); // Remove da lista
                    livrosDictionary.Remove(isbn);       // Remove do dicionário
                    Console.WriteLine($"Livro com ISBN '{isbn}' ('{livroParaRemover.Titulo}') removido com sucesso!");
                }
                else
                {
                    Console.WriteLine($"Erro: Livro com o ISBN '{isbn}' nao encontrado.");
                }
            }
        }
    }
}
