using System;
using System.Threading.Tasks;
using HousingRepairsOnlineApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HousingRepairsOnlineApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SorTriageMap([FromQuery] string emergencyValue, [FromQuery] string notEligibleNonEmergencyValue)
        {
            var wallsFloorAndCeiling = new SorConfiguration ( value: "wallsFloorsCeiling", display: "Walls, floor or ceiling, excluding damp" , options: new SorConfiguration[0]);
            var sink = new SorConfiguration ( value: "sink", display: "Sink, including taps and drainage", options: new SorConfiguration[0]);
            var damagedOrStuckDoors = new SorConfiguration ( value: "damagedOrStuckDoors", display: "Damaged or stuck doors" , options: new SorConfiguration[0]);
            var electricsLightsSwitches = new SorConfiguration (value: "electricsLightsSwitches", display: "Electrics, including lights and switches", options: new SorConfiguration[0]);
            var windows = new SorConfiguration ( value: "windows", display: "Damaged or stuck windows", options: new SorConfiguration[]
            {
                new() { Value = "smashed", Display = "Smashed window(s)"},
                new() { Value = "stuckOpen", Display = "Window stuck open"},
                new() { Value = "stuckShut", Display = "Window stuck shut"},
                new() { Value = "condensation", Display = "Condensation"},
            });
            var dampOrMould = new SorConfiguration ( value: "dampOrMould", display: "Damp or mould", options: new SorConfiguration[]
            {
                new() { Value = "emergency", Display = "Damp or mould caused by a leak"},
                new() { Value = "dampOrMould", Display = "Damp or mould caused by something else"},
            });
            var heatingOrHotWater = new SorConfiguration ( value: "heatingOrHotWater", display: "Heating or hot water", options: new SorConfiguration[0]);
            var heating = new SorConfiguration ( value: "heating", display: "Heating", options: new SorConfiguration[0]);

            var result = new SorConfiguration[]
            {
                new()
                {
                    Display = "Kitchen",
                    Value = "kitchen",
                    Options = new[]
                    {
                        new SorConfiguration(display: "Cupboards, including damaged cupboard doors", value: "cupboards",
                            options: new[] {
                                new SorConfiguration{Display = "Hanging Door1", Value = "doorHangingOff"},
                                new SorConfiguration{Display = "Missing Door2", Value = "doorMissing"},
                            }
                        ),
                        new SorConfiguration( value: "electrical", display: "Electrical, including extractor fans and lightbulbs", options: new SorConfiguration[0]),
                        new SorConfiguration{ Value= "worktop", Display = "Damaged worktop"},
                        heatingOrHotWater,
                        new SorConfiguration( value: "door", display:   "Damaged or stuck doors", options: new SorConfiguration[0]),
                        wallsFloorAndCeiling,
                        sink,
                        windows,
                        dampOrMould,
                    }
                },
                new()
                {
                    Display = "Bathroom",
                    Value = "bathroom",
                    Options = new[]
                    {
                        new SorConfiguration(display: "Bath, including taps", value: "bath",
                            options: new[]
                            {
                                new SorConfiguration{Display = "Bath Taps", Value = "bathTaps"},
                                new SorConfiguration{Display = "Seal Around Bath", Value = "sealAroundBath"},
                            }),
                        windows,
                    }
                },
                new()
                {
                    Display = "Bedroom",
                    Value = "bedroom",
                    Options = new SorConfiguration[0],
                },
                new()
                {
                    Display = "Living Areas",
                    Value = "livingAreas",
                    Options = new[]
                    {
                        new SorConfiguration(display: "Damaged or stuck doors", value: "door",
                            options: new SorConfiguration[]
                            {
                                new() { Value =  "internalDoorIssue", Display = "Internal door issue, including hinges, handle, sticking"},
                                new() { Value =  notEligibleNonEmergencyValue, Display = "Lock on the door"},
                                // new() { Value =  "lockOnDoor", Display = "Lock on the door"},
                                new() { Value =  emergencyValue, Display = "Adjusting a door after a carpet fitting"},
                                // new() { Value =  "adjustingDoorAfterCarpetFitting", Display = "Adjusting a door after a carpet fitting"},
                            })
                    }
                },
                new()
                {
                    Display = "Outside",
                    Value = "outside",
                }
            };
            return Ok(result);
        }
    }
}
