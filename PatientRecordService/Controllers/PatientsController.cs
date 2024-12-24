using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using PatientRecordService.Commands;
using PatientRecordService.Queries;
using PatientRecordService.Handlers.CommandHandlers;
using PatientRecordService.Handlers.QueryHandlers;
using PatientRecordService.Models;

namespace PatientRecordService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly AddPatientHandler _addPatientHandler;
        private readonly GetAllPatientsHandler _getAllPatientsHandler;
        private readonly GetPatientByIdHandler _getPatientByIdHandler;
        private readonly UpdatePatientHandler _updatePatientHandler;
        private readonly DeletePatientHandler _deletePatientHandler;

        public PatientsController(
            AddPatientHandler addPatientHandler,
            GetAllPatientsHandler getAllPatientsHandler,
            GetPatientByIdHandler getPatientByIdHandler,
            UpdatePatientHandler updatePatientHandler,
            DeletePatientHandler deletePatientHandler)
        {
            _addPatientHandler = addPatientHandler;
            _getAllPatientsHandler = getAllPatientsHandler;
            _getPatientByIdHandler = getPatientByIdHandler;
            _updatePatientHandler = updatePatientHandler;
            _deletePatientHandler = deletePatientHandler;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(AddPatientCommand command)
        {
            await _addPatientHandler.Handle(command);
            return Ok("Patient added successfully");
        }

        [HttpGet("getallpatients")]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllPatientsQuery();
            var patients = await _getAllPatientsHandler.Handle(query);
            return Ok(patients);
        }

        [HttpGet("patient/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetPatientByIdQuery { PatientId = id };
            var patient = await _getPatientByIdHandler.Handle(query);

            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpPut("patient/{id}")]
        public async Task<IActionResult> Update(string id, UpdatePatientCommand command)
        {
            command.PatientId = id;
            await _updatePatientHandler.Handle(command);
            return Ok("Patient updated successfully");
        }

        [HttpDelete("patient/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var command = new DeletePatientCommand { PatientId = id };
            await _deletePatientHandler.Handle(command);
            return Ok("Patient deleted successfully");
        }
    }
}
