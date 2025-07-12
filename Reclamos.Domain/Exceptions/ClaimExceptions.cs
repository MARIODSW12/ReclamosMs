

namespace Reclamos.Domain.Exceptions
{
    public class InvalidStatusException : Exception
    {
        public InvalidStatusException() : base("Estado de reclamo invalido") { }
    }
    public class InvalidDescriptionException : Exception
    {
        public InvalidDescriptionException() : base("La descripcion no puede estar vacia") { }
    }

    public class InvalidIdException : Exception
    {
        public InvalidIdException() : base("El id del reclamo es invalido") { }
    }
    public class InvalidMotiveException : Exception
    {
        public InvalidMotiveException() : base("El motivo del reclamo no puede estar vacio") { }
    }
    public class InvalidSolutionException : Exception
    {
        public InvalidSolutionException() : base("La solucion del reclamo no puede estar vacia") { }
    }
    public class InvalidSolutionDetailException : Exception
    {
        public InvalidSolutionDetailException() : base("El detalle de la solucion del reclamo no puede estar vacio") { }
    }
    public class InvalidEvidenceException : Exception
    {
        public InvalidEvidenceException() : base("La evidencia no puede estar vacia") { }
    }
    public class InvalidUserIdException : Exception
    {
        public InvalidUserIdException() : base("El id del usuario es invalido") { }
    }
    public class InvalidAuctionIdException : Exception
    {
        public InvalidAuctionIdException() : base("El id de la subasta es invalido") { }
    }
    public class InvalidPrizeClaimIdException : Exception
    {
        public InvalidPrizeClaimIdException() : base("El id del premio es invalido") { }
    }
    public class MongoDBConnectionException : Exception 
    {
        public MongoDBConnectionException() : base("Error al conectar con la base de datos de mongo") { }
    }

    public class MongoDBUnnexpectedException : Exception
    {
        public MongoDBUnnexpectedException() : base("Error inesperado con la base de datos de mongo") { }
    }
}
