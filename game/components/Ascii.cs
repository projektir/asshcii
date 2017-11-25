using System.Text;

namespace ashhcii {
    public class Ascii : Component {
        public char[,] Characters { get; }

        public Ascii(char[,] characters) {
            Characters = characters;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < Characters.GetLength(0); i++) {
                for (int j = 0; j < Characters.GetLength(1); j++) {
                    stringBuilder.Append(Characters[i, j]);
                }

                if (i < Characters.GetLength(0) - 1) {
                    stringBuilder.Append(" / ");
                }
            }

            return stringBuilder.ToString();
        }
    } 
}
