using System.IO;

namespace Type_faster.Model
{
    public class ScoreRepository
    {
        private readonly string _folder;

        public ScoreRepository(string folderPath)
        {
            _folder = folderPath;
        }

        public int LoadBestScore()
        {
            string path = Path.Combine(_folder, "best_score.txt");
            if (!File.Exists(path)) return 0;
            return int.TryParse(File.ReadAllText(path).Trim(), out int s) ? s : 0;
        }

        public void SaveBestScore(int score)
        {
            File.WriteAllText(Path.Combine(_folder, "best_score.txt"), score.ToString());
        }
    }
}
