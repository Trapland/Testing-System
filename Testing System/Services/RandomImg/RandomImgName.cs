using Testing_System.Services.Hash;

namespace Testing_System.Services.RandomImg
{
    public class RandomImgName : IRandomImgName
    {
        private readonly IHashService _hashService;

        public RandomImgName(IHashService hashService)
        {
            _hashService = hashService;
        }

        public String RandomNameImg(String FileName)
        {
            String savedName = null!;
            String ext = Path.GetExtension(FileName);
            savedName = _hashService.Hash(FileName + DateTime.Now + System.Random.Shared.Next())[..16] + ext;
            return savedName;
        }
    }
}
