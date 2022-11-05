﻿using l99.driver.fanuc.strategies;

namespace l99.driver.fanuc.collectors
{
    public class StateData : FanucMultiStrategyCollector
    {
        public StateData(FanucMultiStrategy strategy) : base(strategy)
        {
            
        }
        
        public override async Task InitPathsAsync()
        {
            await strategy.Apply(typeof(fanuc.veneers.StateData), "state", isCompound: true);
        }
        
        public override async Task CollectRootAsync()
        {
            await strategy.SetNative("poweron_time_min", 
                await strategy.Platform.RdParamDoubleWordNoAxisAsync(6750));
            
            await strategy.SetNative("operating_time_min", 
                await strategy.Platform.RdParamDoubleWordNoAxisAsync(6752));
            
            await strategy.SetNative("cutting_time_min", 
                await strategy.Platform.RdParamDoubleWordNoAxisAsync(6754));
        }
        
        public override async Task CollectForEachPathAsync(short current_path, string[] axis, string[] spindle, dynamic path_marker)
        {
            await strategy.Peel("state",
                await strategy.SetNativeKeyed($"stat_info", 
                    await strategy.Platform.StatInfoAsync()),
                strategy.Get("poweron_time_min"),
                strategy.Get("operating_time_min"),
                strategy.Get("cutting_time_min"),
                await strategy.SetNativeKeyed($"feed_override", 
                    await strategy.Platform.RdPmcRngGByteAsync(12)),
                await strategy.SetNativeKeyed($"rapid_override", 
                    await strategy.Platform.RdPmcRngGByteAsync(14)),
                await strategy.SetNativeKeyed($"spindle_override", 
                    await strategy.Platform.RdPmcRngGByteAsync(30)),
                await strategy.SetNativeKeyed($"modal_m1", 
                    await strategy.Platform.ModalAsync(106,0,3)),
                await strategy.SetNativeKeyed($"modal_m2", 
                    await strategy.Platform.ModalAsync(125,0,3)),
                await strategy.SetNativeKeyed($"modal_m3", 
                    await strategy.Platform.ModalAsync(126,0,3)),
                await strategy.SetNativeKeyed($"modal_t", 
                    await strategy.Platform.ModalAsync(108,0,3)));
        }
    }
}