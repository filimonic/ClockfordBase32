<# 
    Print Clockford's Base32 forward and backward conversion tables
#>

# Clovkford's Base32 Table
# first letter is used for encode
# all letters are used for decode
$clockfordBase32Table = '0oO,1iIlL,2,3,4,5,6,7,8,9,Aa,Bb,Cc,Dd,Ee,Ff,Gg,Hh,Jj,Kk,Mm,Nn,Pp,Qq,Rr,Ss,Tt,Vv,Ww,Xx,Yy,Zz'.Split(",".ToCharArray(),[System.StringSplitOptions]::RemoveEmptyEntries)

# Create forward and reverse hashtables
$fwd_ht = $null
$rev_ht = $null
$fwd_ht = [System.Collections.Hashtable]::new();
$rev_ht = [System.Collections.Hashtable]::new();
for ($i = 0 ; $i -lt $clockfordBase32Table.Count; $i++)
{
    $ctValue = $clockfordBase32Table[$i];
    $fwd_ht.Add($i, $ctValue[0])
    for ($j = $ctValue.Length - 1; $j -ge 0; $j--)
    {
        $rev_ht.Add([char]$ctValue[$j], $i)
    }
}

# Print forward table (Encode table)

$fwd_text = 'internal static readonly char[] clockfordBase32Table_fwd = { '
Write-Host -NoNewline $fwd_text;
$lb = 8;
for ($i = 0; $i -lt 32; $i++) 
{

    Write-Host -NoNewline "'$($fwd_ht[$i])'"
    Write-Host -NoNewline ", "
    $lb--
    if (($lb -eq 0) -and ($i -ne 31)) 
    {
        $lb = 8
        Write-Host "";
        Write-Host -NoNewline (" " * $fwd_text.Length) 
    }
}

# Print padding character
Write-Host -NoNewline "'='"

Write-Host " };"
Write-Host ""


# Print backward table (Decode table)

$rev_text = 'internal static readonly byte[] clockfordBase32Table_rev = { '
$err_id = 255;
$lb = 8;
$txtLine = '';
Write-Host -NoNewline $rev_text;
for ($i = 0; $i -lt 256; $i++) 
{
    $b = [byte]$i
    $c = [char]$b
    $v = 255
    if ($rev_ht.ContainsKey($c))
    {
        $v = $rev_ht[$c]
        $txtLine += "$($c)"
    }
    else
    {
        $txtLine += "_"
    }
    
    Write-Host -NoNewline "0x$([System.Convert]::ToString($v,16).padLeft(2,'0'))"
    if ($i -ne 255) 
    {
        Write-Host -NoNewline ', '
        $txtLine += ', '
    }
    $lb--;
    if (($lb -eq 0) -and ($i -ne 255))
    {
        $lb = 8
        Write-Host "  // $($txtLine)";
        $local:txtLine = ''
        Write-Host -NoNewline (" " * $rev_text.Length) 
    }
}

Write-Host " }; // $($txtLine)"
Write-Host ""