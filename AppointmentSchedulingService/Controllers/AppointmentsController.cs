using AppointmentSchedulingService.Commands;
using AppointmentSchedulingService.Handlers.CommandHandlers;
using AppointmentSchedulingService.Handlers.QueryHandlers;
using AppointmentSchedulingService.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly BookAppointmentHandler _bookHandler;
    private readonly UpdateAppointmentHandler _updateHandler;
    private readonly CancelAppointmentHandler _cancelHandler;
    private readonly GetAppointmentsByDoctorHandler _doctorHandler;
    private readonly GetAppointmentsByDateHandler _dateHandler;

    public AppointmentsController(
        BookAppointmentHandler bookHandler,
        UpdateAppointmentHandler updateHandler,
        CancelAppointmentHandler cancelHandler,
        GetAppointmentsByDoctorHandler doctorHandler,
        GetAppointmentsByDateHandler dateHandler)
    {
        _bookHandler = bookHandler;
        _updateHandler = updateHandler;
        _cancelHandler = cancelHandler;
        _doctorHandler = doctorHandler;
        _dateHandler = dateHandler;
    }

    // POST: api/appointments/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateAppointment([FromBody] BookAppointmentCommand command)
    {
        await _bookHandler.Handle(command);
        return Ok("Appointment booked successfully.");
    }

    // PUT: api/appointments/update/{appointmentId}
    [HttpPut("update/{appointmentId}")]
    public async Task<IActionResult> UpdateAppointment(string appointmentId, [FromBody] UpdateAppointmentCommand command)
    {
        command.AppointmentId = appointmentId;
        await _updateHandler.Handle(command);
        return Ok("Appointment updated successfully.");
    }

    // DELETE: api/appointments/cancel/{appointmentId}
    [HttpDelete("cancel/{appointmentId}")]
    public async Task<IActionResult> CancelAppointment(string appointmentId)
    {
        var command = new CancelAppointmentCommand { AppointmentId = appointmentId };
        await _cancelHandler.Handle(command);
        return Ok("Appointment cancelled successfully.");
    }

    // GET: api/appointments/doctor/{doctorId}
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetAppointmentsByDoctor(string doctorId)
    {
        var query = new GetAppointmentsByDoctorQuery { DoctorId = doctorId };
        var appointments = await _doctorHandler.Handle(query);
        return Ok(appointments);
    }

    // GET: api/appointments/date
    [HttpGet("appointmentsbydate")]
    public async Task<IActionResult> GetAppointmentsByDate([FromQuery] DateTime date)
    {
        var query = new GetAppointmentsByDateQuery { AppointmentDate = date };
        var appointments = await _dateHandler.Handle(query);
        return Ok(appointments);
    }
}
