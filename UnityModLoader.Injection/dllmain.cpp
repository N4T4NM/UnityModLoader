#include <Windows.h>
#include <process.h>
#include <mono/jit/jit.h>

DWORD GetMonoFunction(HMODULE hMono, const char* fname) {
    return (DWORD)GetProcAddress(hMono, fname);
}

UINT __stdcall dwMain2(void*) {

	//Check for mono or mono2.0
	HMODULE hMono = GetModuleHandleA("mono.dll");
	if (hMono == 0)
		hMono = GetModuleHandleA("mono-2.0-bdwgc.dll");

	//Attach
	typedef MonoThread* (__cdecl* mono_thread_attach_t)(MonoDomain* mDomain);
	mono_thread_attach_t mono_thread_attach = (mono_thread_attach_t)GetMonoFunction(hMono, "mono_thread_attach");

	//Class
	typedef MonoClass* (__cdecl* mono_class_from_name_t)(MonoImage* image, const char* name_space, const char* name);
	typedef MonoMethod* (__cdecl* mono_class_get_method_from_name_t)(MonoClass* mclass, const char* name, int param_count);
	mono_class_from_name_t mono_class_from_name = (mono_class_from_name_t)GetMonoFunction(hMono, "mono_class_from_name");
	mono_class_get_method_from_name_t mono_class_get_method_from_name = (mono_class_get_method_from_name_t)GetMonoFunction(hMono, "mono_class_get_method_from_name");

	//Code execution
	typedef MonoObject* (__cdecl* mono_runtime_invoke_t)(MonoMethod* method, void* obj, void** params, MonoObject** exc);
	mono_runtime_invoke_t mono_runtime_invoke = (mono_runtime_invoke_t)GetMonoFunction(hMono, "mono_runtime_invoke");

	//Assembly
	typedef MonoAssembly* (__cdecl* mono_assembly_open_t)(MonoDomain* mDomain, const char* filepath);
	typedef MonoImage* (__cdecl* mono_assembly_get_image_t)(MonoAssembly* assembly);
	mono_assembly_open_t mono_assembly_open_ = (mono_assembly_open_t)GetMonoFunction(hMono, "mono_domain_assembly_open");
	mono_assembly_get_image_t mono_assembly_get_image_ = (mono_assembly_get_image_t)GetMonoFunction(hMono, "mono_assembly_get_image");

	//Domain
	typedef MonoDomain* (__cdecl* mono_root_domain_get_t)();
	typedef MonoDomain* (__cdecl* mono_domain_get_t)();
	mono_root_domain_get_t mono_root_domain_get = (mono_root_domain_get_t)GetMonoFunction(hMono, "mono_get_root_domain");
	mono_domain_get_t mono_domain_getnormal = (mono_domain_get_t)GetMonoFunction(hMono, "mono_domain_get");

	//No clue what happens here, but is required in order for the domain to be ready at time for code-execution.
	mono_thread_attach(mono_root_domain_get());
	//Now that we're attached we get the domain we are in.
	MonoDomain* domain = mono_domain_getnormal();
	//Opening a custom assembly in the domain.
	MonoAssembly* domainassembly = mono_assembly_open_(domain, "./UnityModLoader.Library.dll");
	//Getting the assemblys Image(Binary image, essentially a file-module).
	MonoImage* Image = mono_assembly_get_image_(domainassembly);
	//Declaring the class inside the custom assembly we're going to use. (Image, NameSpace, ClassName)
	MonoClass* pClass = mono_class_from_name(Image, "UnityModLoader.Core", "ModLoader");
	//Declaring the method, that attaches our assembly to the game. (Class, MethodName, Parameters)
	MonoMethod* MonoClassMethod = mono_class_get_method_from_name(pClass, "StartModLoader", 0);
	//Invoking said method.
	mono_runtime_invoke(MonoClassMethod, NULL, NULL, NULL);

    return 0;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        _beginthreadex(0, 0, dwMain2, 0, 0, 0);
    }
    return TRUE;
}

