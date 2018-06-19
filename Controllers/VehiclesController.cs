using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vega.Controllers.Resources;
using vega.Models;
using vega.Persistence;

namespace vega.Controllers
{

    [Route("/api/vehicles")]
    public class VehiclesController : Controller
    {
        private readonly IMapper mapper;
        private readonly VegaDbContext context;
        private readonly IVehicleRepository vehicleRepository;
        public VehiclesController(IMapper mapper, VegaDbContext context, IVehicleRepository vehicleRepository)
        {
            this.vehicleRepository = vehicleRepository;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody]SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // var model = await context.Models.FindAsync(vehicleResource.ModelId);
            // if(model == null)
            // {
            //     ModelState.AddModelError("ModelId", "Invalid ModelId.");
            //     return BadRequest(ModelState);
            // }

            // In order to create a vehicle instance into database
            // input data type: VehicleResource
            // output data type: Vehicle
            // so we need to map VehicleResource to Vehicle
            var vehicle = mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource);
            vehicle.UpdateTime = DateTime.Now;
            vehicleRepository.Add(vehicle);
            await context.SaveChangesAsync();

            // await context.Models.Include(m => m.Make).SingleOrDefaultAsync(m => m.Id == vehicle.ModelId);
            vehicle = await vehicleRepository.GetVehicle(vehicle.Id);

            var result = mapper.Map<Vehicle, VehicleResource>(vehicle);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody]SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // var vehicle = await context.Vehicles.Include(v => v.Features).SingleOrDefaultAsync(v => v.Id == id);
            var vehicle = await vehicleRepository.GetVehicle(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource, vehicle);
            vehicle.UpdateTime = DateTime.Now;

            await context.SaveChangesAsync();

            var result = mapper.Map<Vehicle, SaveVehicleResource>(vehicle);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            vehicleRepository.Remove(vehicle);
            await context.SaveChangesAsync();

            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await vehicleRepository.GetVehicle(id, includeRelated: false);

            if (vehicle == null)
            {
                return NotFound();
            }

            var vehicleResource = mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(vehicleResource);
        }
    }
}