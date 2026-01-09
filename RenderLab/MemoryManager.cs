using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace RenderLab
{

    /// <summary>
    /// Simple external-process memory reader. Implements correct handle lifetime management.
    /// </summary>
    public sealed class MemoryManager : IDisposable
    {
        private Process? _process;
        private SafeProcessHandle? _processHandle;

        public bool Attach(string processName)
        {
            Detach(); // ensure clean state if re-attaching

            _process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (_process == null)
                return false;

            _processHandle = Kernel32.OpenProcess(
                Kernel32.ProcessAccessFlags.PROCESS_VM_READ
                | Kernel32.ProcessAccessFlags.PROCESS_VM_WRITE
                | Kernel32.ProcessAccessFlags.PROCESS_VM_OPERATION
                | Kernel32.ProcessAccessFlags.PROCESS_QUERY_INFORMATION,
                bInheritHandle: false,
                dwProcessId: _process.Id);

            if (_processHandle is null || _processHandle.IsInvalid)
            {
                Detach();
                return false;
            }

            return true;
        }

        public bool IsAttached => _process != null && _processHandle != null && !_processHandle.IsInvalid && !_processHandle.IsClosed;

        public bool Detach()
        {
            try
            {
                _processHandle?.Dispose();
                _processHandle = null;

                _process?.Dispose();
                _process = null;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IntPtr GetProcessBase()
        {
            EnsureAttached();
            // MainModule can throw if access is denied; surface that to caller.
            return _process!.MainModule!.BaseAddress;
        }

        public IntPtr GetProcessModuleBase(string moduleName)
        {
            EnsureAttached();

            // Module enumeration can throw if insufficient privileges or cross-bitness.
            var mod = _process!.Modules.Cast<ProcessModule>()
                .FirstOrDefault(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));

            return mod?.BaseAddress ?? IntPtr.Zero;
        }

        public byte[] ReadBytes(IntPtr address, int length)
        {
            EnsureAttached();

            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            var buffer = new byte[length];

            if (!Kernel32.ReadProcessMemory(_processHandle!, address, buffer, buffer.Length, out var bytesRead))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "ReadProcessMemory failed.");

            if (bytesRead.ToInt64() != length)
                throw new InvalidOperationException($"ReadProcessMemory read {bytesRead} bytes, expected {length}.");

            return buffer;
        }

        public byte ReadByte(IntPtr address) => ReadBytes(address, 1)[0];

        public int ReadInt32(IntPtr address)
        {
            var buffer = ReadBytes(address, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public long ReadInt64(IntPtr address)
        {
            var buffer = ReadBytes(address, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public float ReadFloat(IntPtr address)
        {
            var buffer = ReadBytes(address, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadDouble(IntPtr address)
        {
            var buffer = ReadBytes(address, 8); // FIX: double is 8 bytes
            return BitConverter.ToDouble(buffer, 0);
        }

        public IntPtr ReadAddress(IntPtr address)
        {
            var buffer = ReadBytes(address, IntPtr.Size); // FIX: read once and use it
            return IntPtr.Size == 4
                ? new IntPtr(BitConverter.ToInt32(buffer, 0))
                : new IntPtr(BitConverter.ToInt64(buffer, 0));
        }

        /// <summary>
        /// Reads a string of at most maxBytes. If nullTerminated is true, stops at first '\0'.
        /// Default encoding is UTF-16 (Windows "Unicode").
        /// </summary>
        public string ReadString(IntPtr address, int maxBytes, Encoding? encoding = null, bool nullTerminated = true)
        {
            EnsureAttached();

            if (maxBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxBytes));

            encoding ??= Encoding.Unicode;

            var buffer = ReadBytes(address, maxBytes);

            int used = buffer.Length;
            if (nullTerminated)
            {
                if (encoding == Encoding.Unicode || encoding == Encoding.BigEndianUnicode)
                {
                    // look for 2-byte null
                    for (int i = 0; i + 1 < buffer.Length; i += 2)
                    {
                        if (buffer[i] == 0 && buffer[i + 1] == 0)
                        {
                            used = i;
                            break;
                        }
                    }
                }
                else
                {
                    // look for 1-byte null
                    int idx = Array.IndexOf(buffer, (byte)0);
                    if (idx >= 0) used = idx;
                }
            }

            return encoding.GetString(buffer, 0, used);
        }

        private void EnsureAttached()
        {
            if (!IsAttached)
                throw new InvalidOperationException("Not attached to a process. Call Attach() first.");
        }

        public void Dispose()
        {
            Detach();
            GC.SuppressFinalize(this);
        }
    }
}
