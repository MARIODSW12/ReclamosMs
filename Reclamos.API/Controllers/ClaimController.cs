using System.Text.Json;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using Reclamos.Application.Commands;
using Reclamos.Application.DTOs;
using Reclamos.Domain.Events;
using Reclamos.Domain.ValueObjects;
using Reclamos.Infrastructure.Interfaces;
using Reclamos.Infrastructure.Queries;
using Reclamos.Infrastructure.Services;
using RestSharp;
using ZstdSharp.Unsafe;
using static System.Net.Mime.MediaTypeNames;


namespace Reclamos.Presentation.Controllers
{
    [ApiController]
    [Route("api/claim")]
    public class ClaimController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRestClient _restClient;
        private readonly ICloudinaryService _cloudinaryService;

        public ClaimController(IMediator mediator, IPublishEndpoint publishEndpoint, ICloudinaryService cloudinaryService, IRestClient restClient)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        #region CreateClaim
        [HttpPost("createClaim")]
        public async Task<IActionResult> CreateClaim([FromForm] CreateClaimDto claimDto, [FromForm] string textEvidence, List<IFormFile> evidences)
        {

            try
            {
                List<string> saveEvidences = [textEvidence];
                foreach (var file in evidences)
                {
                    using var stream = file.OpenReadStream();
                    Console.WriteLine($"Subiendo archivo: {file.FileName}");
                    var url = await _cloudinaryService.SubirArchivo(stream, file.FileName);
                    Console.WriteLine($"Archivo subido: {url}");
                    saveEvidences.Add(url);
                }
                var claimId = await _mediator.Send(new CreateClaimCommand(claimDto, saveEvidences, null));

                if (claimId == null)
                {
                    return BadRequest("No se pudo crear el reclamo.");
                }


                return CreatedAtAction(nameof(CreateClaim), new { id = claimId }, new
                {
                    id = claimId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el reclamo: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
        #endregion

        #region SolveClaim
        [HttpPost("solveClaim")]
        public async Task<IActionResult> SolveClaim([FromBody] SolveClaimDto claimDto)
        {

            try
            {
                var claim = await _mediator.Send(new GetClaimPorIdQuery(claimDto.ClaimId));
                if (claim == null)
                {
                    return NotFound($"No se encontró el reclamo con ID {claimDto.ClaimId}.");
                }

                var claimId = await _mediator.Send(new SolveClaimCommand(claimDto));

                if (claimId == null)
                {
                    return BadRequest("No se pudo solventar el reclamo.");
                }

                await _publishEndpoint.Publish(new NotificationSendEvent(
                    [claim.UserId],
                    "Reclamo Solventado",
                    $"El reclamo con ID {claimId} ha sido solventado. Se ha tomado la siguiente decision: {claimDto.Solution}; {claimDto.SolutionDetail}"));

                return CreatedAtAction(nameof(SolveClaim), new { id = claimId }, new
                {
                    id = claimId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al solventar el reclamo: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
        #endregion

        #region OpenClaim
        [HttpPost("openClaim/{id}")]
        public async Task<IActionResult> OpenClaim([FromRoute] string id)
        {

            try
            {
                var claimId = await _mediator.Send(new OpenClaimCommand(id));

                if (claimId == null)
                {
                    return BadRequest("No se pudo abrir el reclamo.");
                }


                return CreatedAtAction(nameof(OpenClaim), new { id = claimId }, new
                {
                    id = claimId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir el reclamo: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
        #endregion

        #region GetClaimById
        [HttpGet("getClaimById/{id}")]
        public async Task<IActionResult> GetClaimById([FromRoute] string id)
        {
            try
            {
                var claim = await _mediator.Send(new GetClaimPorIdQuery(id));

                if (claim == null)
                {
                    return NotFound($"No se encontró el reclamo con ID {id}.");
                }

                return Ok(claim);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el reclamo: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
        #endregion

        #region GetClaimsByStatus
        [HttpGet("getClaimsByStatus/{status}")]
        public async Task<IActionResult> GetClaimsByStatus([FromRoute] string status)
        {
            try
            {
                var claims = await _mediator.Send(new GetTodosLosClaimPorStatusQuery(status));

                return Ok(claims);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los reclamos: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
        #endregion

        #region CreateClaimPrize
        [HttpPost("createClaimPrize")]
        public async Task<IActionResult> CreateClaimPrize([FromForm] CreateClaimDto claimDto, [FromForm] string textEvidence, List<IFormFile> evidences)
        {

            try
            {
                var APIRequest = new RestRequest(Environment.GetEnvironmentVariable("AUCTION_MS_URL") + "/getPrizeClaim/" + claimDto.AuctionId + "/" + claimDto.UserId, Method.Get);
                var APIResponse = await _restClient.ExecuteAsync(APIRequest);
                if (!APIResponse.IsSuccessful)
                {
                    Console.WriteLine(APIResponse.Content);
                    return BadRequest("No hay un reclamo de premio de ese usuario en esa subasta.");
                }
                var prizeClaim = JsonDocument.Parse(APIResponse.Content).Deserialize<GetPrizeClaimDto>();
                if (prizeClaim == null)
                {
                    return BadRequest("No hay un reclamo de premio de ese usuario en esa subasta.");
                }
                List<string> saveEvidences = [textEvidence];
                foreach (var file in evidences)
                {
                    using var stream = file.OpenReadStream();
                    Console.WriteLine($"Subiendo archivo: {file.FileName}");
                    var url = await _cloudinaryService.SubirArchivo(stream, file.FileName);
                    Console.WriteLine($"Archivo subido: {url}");
                    saveEvidences.Add(url);
                }
                var claimId = await _mediator.Send(new CreateClaimCommand(
                    claimDto, saveEvidences, prizeClaim.id));

                if (claimId == null)
                {
                    return BadRequest("No se pudo crear el reclamo.");
                }
                var APIRequestUsers = new RestRequest(Environment.GetEnvironmentVariable("USER_MS_URL") + "/allUsers", Method.Get);
                var APIResponseUsers = await _restClient.ExecuteAsync(APIRequestUsers);
                if (APIResponseUsers.IsSuccessful)
                {
                    try
                    {
                        var users = JsonDocument.Parse(APIResponseUsers.Content).Deserialize<List<UserDto>>();
                        if (users != null && users.Count > 0)
                        {
                            await _publishEndpoint.Publish(new NotificationSendEvent(
                                users.Where(u => u.roleId == "6818bd0b035415cfcd8aa238").Select(u => u.id).ToList(),
                                "Reclamo de Premio Creado",
                                $"Se ha creado un reclamo de premio para la subasta {claimDto.AuctionId} por parte del usuario {claimDto.UserId}."));
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    
                }

                return CreatedAtAction(nameof(CreateClaim), new { id = claimId }, new
                {
                    id = claimId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el reclamo: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }
        #endregion
    }

}
