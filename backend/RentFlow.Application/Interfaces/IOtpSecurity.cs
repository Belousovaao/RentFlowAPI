namespace RentFlow.Application.Interfaces;

public interface IOtpSecurity
{
    string GenerateCode();
    string HashCode(string code);
    string HashDestination(string destination);
    string HashIp(string ipAddress);
    string HashIpDestination(
        string ipAddress,
        string destination);
}