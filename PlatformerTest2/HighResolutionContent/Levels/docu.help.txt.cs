/// <summary>
        /// ��������� ������� ��� ����� � ��� ���������.
        /// </summary>
        /// <param name="tileType">
        /// ������ �� ����� �����, ������������ ��� �����.
        /// </param>
        /// <param name="x">
        /// ��������� ����� ������ �� ��� X.
        /// </param>
        /// <param name="y">
        /// ��������� ����� ������ �� ��� Y.
        /// </param>
        /// <returns>���������� ����.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            if (tileType >= 'A' && tileType < 'I')
            {
                // ������ �����
                return LoadEnemyTile(x, y, "Monster"+tileType);
            }

            switch (tileType)
            {
                // �����
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // �����
                case 'X':
                    return LoadExitTile(x, y);

                // �������
                case 'I':
                    return LoadItemTile(x, y);

                // ��������� � �������
                case '-':
                    return LoadVarietyTile("Platform", 5, TileCollision.Platform);


                // ���� - ���������
                case '~':
                    return LoadVarietyTile("BlockB", 10, TileCollision.Platform);

                // ���������� ����
                case ':':
                    return LoadVarietyTile("BlockB", 10, TileCollision.Passable);

                // ��������� ������� ��� ������ 1
                case '1':
                    return LoadStartTile(x, y);

                // ������������ ����
                case '#':
                    return LoadVarietyTile("BlockA", 10, TileCollision.Impassable);

                // ���������, ���� ������
                case '>':
                    return LoadTile("HelpRunRight", TileCollision.Passable);


                // ����������� ������!
                default:
                    throw new NotSupportedException(String.Format("���������� ������ '{0}'! ����������: {1}, {2}.", tileType, x, y));
            }
        }