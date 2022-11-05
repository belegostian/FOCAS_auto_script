﻿using l99.driver.fanuc.strategies;

namespace l99.driver.fanuc.collectors
{
    public class ProductionData : FanucMultiStrategyCollector
    {
        public ProductionData(FanucMultiStrategy strategy) : base(strategy)
        {
            
        }
        
        public override async Task InitPathsAsync()
        {
            await strategy.Apply(typeof(fanuc.veneers.ProductionData), "production", isCompound: true);
        }
        
        public override async Task CollectForEachPathAsync(short current_path, string[] axis, string[] spindle, dynamic path_marker)
        {
            // cnc_rdprgnum
            await strategy.SetNativeKeyed($"program_number",
                await strategy.Platform.RdPrgNumAsync());
            var b = strategy.GetKeyed($"program_number");
            
            // cnc_exeprgname
            await strategy.SetNativeKeyed($"program_name", 
                await strategy.Platform.ExePrgNameAsync());
            
            // not supported on 16i
            //var o_num = strategy.GetKeyed($"program_name")
            //    .response.cnc_exeprgname.exeprg.o_num;
            
            var running_num = strategy.GetKeyed($"program_number")
                .response.cnc_rdprgnum.prgnum.data;
            
            var main_num = strategy.GetKeyed($"program_number")
                .response.cnc_rdprgnum.prgnum.mdata;
            
            //System.Console.WriteLine($"{this.strategy.Machine.Id} {current_path} {b.response.cnc_rdprgnum.prgnum.data} {b.response.cnc_rdprgnum.prgnum.mdata}");
            
            await strategy.Peel("production",
                strategy.GetKeyed($"program_name"),
                strategy.GetKeyed("program_number"),
                await strategy.SetNativeKeyed($"prog_dir", 
                    await strategy.Platform.RdProgDir3Async(running_num)),
                await strategy.SetNativeKeyed($"prog_dir", 
                    await strategy.Platform.RdProgDir3Async(main_num)),
                await strategy.SetNativeKeyed($"pieces_produced", 
                    await strategy.Platform.RdParamDoubleWordNoAxisAsync(6711)),
                await strategy.SetNativeKeyed($"pieces_produced_life", 
                    await strategy.Platform.RdParamDoubleWordNoAxisAsync(6712)),
                await strategy.SetNativeKeyed($"pieces_remaining", 
                    await strategy.Platform.RdParamDoubleWordNoAxisAsync(6713)),
                await strategy.SetNativeKeyed($"cycle_time_min", 
                    await strategy.Platform.RdParamDoubleWordNoAxisAsync(6758)),
                await strategy.SetNativeKeyed($"cycle_time_ms", 
                    await strategy.Platform.RdParamDoubleWordNoAxisAsync(6757)));
        }
    }
}