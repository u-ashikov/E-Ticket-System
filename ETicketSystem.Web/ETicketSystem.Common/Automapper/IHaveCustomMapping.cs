namespace ETicketSystem.Common.Automapper
{
	using AutoMapper;

	public interface IHaveCustomMapping
    {
		void ConfigureMapping(Profile mapper);
    }
}
