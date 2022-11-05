﻿using l99.driver.@base;

namespace l99.driver.fanuc.veneers
{
    public class AxisData : Veneer
    {
        public AxisData(string name = "", bool isCompound = false, bool isInternal = false) : base(name, isCompound, isInternal)
        {
            lastChangedValue = new
            {
                
            };
        }
        
        protected override async Task<dynamic> AnyAsync(dynamic input, params dynamic?[] additionalInputs)
        {
            // skip index 7 - power consumption
            if (additionalInputs.Slice(0,6).All(o => o.success == true))
            {
                var current_axis = input;
                var axes_names = additionalInputs[0];
                var axis_dynamic = additionalInputs[1].response.cnc_rddynamic2.rddynamic;
                var figures = additionalInputs[2].response.cnc_getfigure.dec_fig_in;
                var axes_loads = additionalInputs[3];
                var servo_temp = additionalInputs[4];
                var coder_temp = additionalInputs[5];
                var power = additionalInputs[6];
                var obs_focas_support = additionalInputs[7];
                IEnumerable<dynamic> obs_alarms = additionalInputs[8];
                var prev_axis_dynamic = additionalInputs[9];

                var load_fields = axes_loads.response.cnc_rdsvmeter.loadmeter.GetType().GetFields();
                var load_value = load_fields[current_axis - 1]
                    .GetValue(axes_loads.response.cnc_rdsvmeter.loadmeter);
                
                var axes_fields = axes_names.response.cnc_rdaxisname.axisname.GetType().GetFields();
                var axis_value = axes_fields[current_axis - 1]
                    .GetValue(axes_names.response.cnc_rdaxisname.axisname);
                var axis_name = ((char) axis_value.name).AsAscii() +
                                   ((char) axis_value.suff).AsAscii();

                bool overtravel = false;
                bool overheat = false;
                bool servo = false;

                if (obs_alarms != null)
                {
                    var axis_alarms = obs_alarms
                        .Where(a => a.axis_code == current_axis);

                    overtravel = axis_alarms.Any(a => a.type == "OT");
                    overheat = axis_alarms.Any(a => a.type == "OH");
                    servo = axis_alarms.Any(a => a.type == "SV");
                }

                bool motion = (prev_axis_dynamic != null && prev_axis_dynamic.success == true) 
                    ? prev_axis_dynamic.response.cnc_rddynamic2.rddynamic.pos.absolute != axis_dynamic.pos.absolute
                    : false;
                
                var current_value = new
                {
                    number = current_axis,
                    name = axis_name,
                    feed = axis_dynamic.actf,
                    feed_eu = "mm/min",
                    load = load_value.data / Math.Pow(10.0, load_value.dec),
                    load_eu = "percent",
                    servo_temp = servo_temp.response.cnc_diagnoss.diag.cdata,
                    servo_temp_eu = "celsius",
                    coder_temp = coder_temp.response.cnc_diagnoss.diag.cdata,
                    coder_temp_eu = "celsius",
                    power = power.response.cnc_diagnoss.diag.ldata,
                    power_eu = "watt",
                    alarms = new
                    {
                        overtravel,
                        overheat,
                        servo
                    },
                    position = new
                    {
                        absolute = axis_dynamic.pos.absolute / Math.Pow(10.0, figures[current_axis-1]),
                        machine = axis_dynamic.pos.machine / Math.Pow(10.0, figures[current_axis-1]),
                        relative = axis_dynamic.pos.relative / Math.Pow(10.0, figures[current_axis-1]),
                        distance = axis_dynamic.pos.distance / Math.Pow(10.0, figures[current_axis-1])
                    },
                    motion
                };

                await onDataArrivedAsync(input, current_value);
                
                if(current_value.IsDifferentString((object)lastChangedValue))
                {
                    await onDataChangedAsync(input, current_value);
                }
            }
            else
            {
                await onErrorAsync(input);
            }

            return new { veneer = this };
        }
    }
}