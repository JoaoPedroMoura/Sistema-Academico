namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>Serviço de domínio que gera uma grade de horário via GRASP (ARCHITECTURE.md §2.3).</summary>
public interface IScheduleGenerator
{
    ScheduleResult Generate(ScheduleGenerationInput input, GraspOptions? options = null);
}
