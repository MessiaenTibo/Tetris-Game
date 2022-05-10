using System;
using System.Linq;
using System.Reflection;

namespace Tetris
{
    public class BlockQueue
    {
        private  Block[] blocks;
        private readonly Random random = new Random();

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {
            PopulateBlocks();
            NextBlock = RandomBlock();
        }

        private void PopulateBlocks()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var types = currentAssembly.GetTypes();
            blocks = types
                .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(Block)))
                .Select(Activator.CreateInstance).Cast<Block>().ToArray();
        }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
    }
}
