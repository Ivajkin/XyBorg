/// <summary>
        /// Загружаем внешний вид тайла и его поведение.
        /// </summary>
        /// <param name="tileType">
        /// Символ из файла карты, обозначающий тип тайла.
        /// </param>
        /// <param name="x">
        /// Положение среди тайлов по оси X.
        /// </param>
        /// <param name="y">
        /// Положение среди тайлов по оси Y.
        /// </param>
        /// <returns>Загруженый тайл.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            if (tileType >= 'A' && tileType < 'I')
            {
                // Разные враги
                return LoadEnemyTile(x, y, "Monster"+tileType);
            }

            switch (tileType)
            {
                // Пусто
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Выход
                case 'X':
                    return LoadExitTile(x, y);

                // Предмет
                case 'I':
                    return LoadItemTile(x, y);

                // Платформа в воздухе
                case '-':
                    return LoadVarietyTile("Platform", 5, TileCollision.Platform);


                // Блок - платформа
                case '~':
                    return LoadVarietyTile("BlockB", 10, TileCollision.Platform);

                // Проходимый блок
                case ':':
                    return LoadVarietyTile("BlockB", 10, TileCollision.Passable);

                // Начальная позиция для игрока 1
                case '1':
                    return LoadStartTile(x, y);

                // Непроходимый блок
                case '#':
                    return LoadVarietyTile("BlockA", 10, TileCollision.Impassable);

                // Подсказка, беги вправо
                case '>':
                    return LoadTile("HelpRunRight", TileCollision.Passable);


                // Неизвестный символ!
                default:
                    throw new NotSupportedException(String.Format("Непонятный символ '{0}'! Координаты: {1}, {2}.", tileType, x, y));
            }
        }