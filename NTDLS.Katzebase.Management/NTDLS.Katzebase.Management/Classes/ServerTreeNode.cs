﻿using NTDLS.Katzebase.Client;
using static NTDLS.Katzebase.Management.Classes.Constants;

namespace NTDLS.Katzebase.Management.Classes
{
    public class ServerTreeNode : TreeNode
    {
        public ServerNodeType NodeType { get; set; }

        public string ServerAddress { get; set; } = string.Empty;
        public int ServerPort { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public KbClient? ServerClient { get; set; }

        public ServerTreeNode(string name) :
             base(name)
        {
        }
    }
}
