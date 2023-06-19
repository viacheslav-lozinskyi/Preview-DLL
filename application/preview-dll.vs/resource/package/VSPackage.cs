using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class PreviewDLL : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string APPLICATION = "Visual Studio";
            public const string COMPANY = "Viacheslav Lozinskyi";
            public const string COPYRIGHT = "Copyright (c) 2020-2023 by Viacheslav Lozinskyi. All rights reserved.";
            public const string DESCRIPTION = "Quick preview of DLL, EXE and SYS files";
            public const string GUID = "6171BB93-BA14-4ECF-B72B-5107D12E5D0F";
            public const string HOST = "MetaOutput";
            public const string NAME = "Preview-DLL";
            public const string VERSION = "1.1.0";
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            extension.AnyPreview.Connect(CONSTANT.APPLICATION, CONSTANT.NAME);
            extension.AnyPreview.Register(".DLL", new preview.VSPreview());
            extension.AnyPreview.Register(".EXE", new preview.VSPreview());
            extension.AnyPreview.Register(".SYS", new preview.VSPreview());
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        protected override int QueryClose(out bool canClose)
        {
            extension.AnyPreview.Disconnect();
            canClose = true;
            return VSConstants.S_OK;
        }
    }
}
