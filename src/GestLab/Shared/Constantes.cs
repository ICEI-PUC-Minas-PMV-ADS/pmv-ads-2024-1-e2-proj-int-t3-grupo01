using GestLab.Models;

namespace GestLab.Shared
{
    public static class Constantes
    {
        public const string SessionKeyID = "_Id";
        public const string SessionKeyNome = "_Nome";
        public const string SessionKeyEmail = "_Email";
        public const string SessionKeyTipo = "_Tipo";
        public const string SessionKeyClienteId = "_ClienteId";
        public const string SessionKeyClienteNome = "_ClienteNome";

        public const string PerfilAdm = "Administrador";
        public const string PerfilMontador = "Montador";
        public const string PerfilCliente = "Cliente";

        public static UsuarioModel UsuarioModel { get; set; }

        public const string StatusPendenteLenteArmacao = "Pendente Lentes e Armação";
        public const string StatusPendenteArmacao = "Pendente Armação";
        public const string StatusPendenteLentes = "Pendente Lentes";
        public const string StatusNovo = "Novo";
        public const string StatusEmMontagem = "Em Montagem";
        public const string StatusAguardandoPagamento = "Aguardando Pagamento";
        public const string StatusFinalizado = "Finalizado";
    }
}
