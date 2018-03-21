import { NgModuleFactoryLoader, InjectionToken, NgModuleFactory, Type, Compiler } from '@angular/core';
/**
 * A map key'd by loadChildren strings and Modules or NgModuleFactories as vaules
 */
export declare type ModuleMap = {
    [key: string]: Type<any> | NgModuleFactory<any>;
};
/**
 * Token used by the ModuleMapNgFactoryLoader to load modules
 */
export declare const MODULE_MAP: InjectionToken<ModuleMap>;
/**
 * NgModuleFactoryLoader which does not lazy load
 */
export declare class ModuleMapNgFactoryLoader implements NgModuleFactoryLoader {
    private compiler;
    private moduleMap;
    constructor(compiler: Compiler, moduleMap: ModuleMap);
    load(loadChildrenString: string): Promise<NgModuleFactory<any>>;
    private loadFactory(factory);
    private loadAndCompile(type);
}
