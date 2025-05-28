# Caminho da pasta onde os arquivos serão verificados
$directoryPath = "./"

# Regex para métodos assíncronos sem o sufixo 'Async'
$regex = [regex]"public\s+async\s+(Task|ValueTask)<?.*?>?\s+(\w+)\s*\("

# Obter todos os arquivos .cs na pasta e subpastas
$files = Get-ChildItem -Path $directoryPath -Recurse -Include *.cs

# Flag para identificar erros
$hasErrors = $false

foreach ($file in $files) {
    Write-Host "Verificando arquivo: $($file.FullName)"

    # Ler todas as linhas do arquivo
    $lines = Get-Content -Path $file.FullName

    foreach ($line in $lines) {
        # Verificar se a linha corresponde à regex
        $matches = $regex.Match($line)
        
        if ($matches.Success -and $matches.Groups[2].Value) {
            $methodName = $matches.Groups[2].Value

            # Verificar se o método não termina com "Async"
            if (-not $methodName.EndsWith("Async")) {
                Write-Host "Erro: Método '$methodName' não termina com 'Async'." -ForegroundColor Red
                $hasErrors = $true
            }
        }
    }
}

if ($hasErrors) {
    Write-Host "Foram encontrados métodos assíncronos que não terminam com 'Async'." -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "Todos os métodos assíncronos estão corretos!"
    exit 0
}
