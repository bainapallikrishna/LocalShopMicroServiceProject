 
using LocalShop.Interface;  
using LocalShop.Models.DTOs;  

public class AuthService : IAuthService  
{  
    private readonly IUserLoginRepository _userRepo;  

    public AuthService(IUserLoginRepository userRepo)  
    {  
        _userRepo = userRepo;  
    }  

    public async Task<bool> ValidateCredentialsAsync(LoginRequest loginRequest)  
    {  
        var user = await _userRepo.GetUserLoginAsync(loginRequest.Username, loginRequest.Password);  
        return user != null && user.Password == loginRequest.Password; // Simplified  
    }  
}  
