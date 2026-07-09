namespace Sistema_Hospitalario.CapaNegocio.DTOs.Comunes
{
    /// <summary>
    /// DTO genérico para ítems de catálogo (roles, estados, tipos) usados en selectores de la UI.
    /// </summary>
    public class CatalogoItemDto
    {
        /// <summary>Identificador único del ítem.</summary>
        public int Id { get; set; }

        /// <summary>Nombre descriptivo del ítem.</summary>
        public string Nombre { get; set; }
    }
}
