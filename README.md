# AdvancedTools_AI 

This repository is all about getting to know with Unity ML Agents.
In this repository I will try to learn an Agent to move to an elevated area and jump on it to reach a goal.

Machine learning is affecting our lifes more than ever, from AI in video games to what you see on your social media feed.
Machine learning is everywhere and its not going away, there for i found it a good idea to get to know more about this subect to better understand the technology. 

To test ML agents, you first have create an agent that can walk around in a small environment. 
In this environment there is a goal to reach for this agent. Once this agent has reach it goal the episode will and and he will get a reward. If the agent runs off the enviorment or when a certain amount of steps has been past, the agent is given a negative reward. You also give the Agent some Observations for the neural network to base it decisions on. Combine all of this and you have an agent that can learn over time to reach a certain goal.

You can create multiple environments at once to speed up training.
For my computer, having 15 environments was the sweet spot.

# The learning

The neural network has a lot of configuration you can do on it. To understand what configurations you can do, you can go to:
https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Training-Configuration-File.md

I decided to start with a config i found on the internet that demonstrated a scenario senario. You can find the artical here: 
https://towardsdatascience.com/an-introduction-to-unity-ml-agents-6238452fcf4c

All of the results can be found at this url:
https://tensorboard.dev/experiment/xOp8Q2MZRvmVm2w0dgk6hw/#scalars
You can also open them offline if you have tensorboard installed. The logdir folder is the results folder.

I first ran the same config 3 times to check how much randomness there was while using the same config. 
You can see these runs as starting_config_moveJump_v1_run_1..3

When looking at the data, you can see that there is a lot of variance in the 3 runs. Looking at run 2, you can see fairly early on that it found a method of getting to the goal. Looking at run 1 it seems to also starting to find something but is not as good as run 2. Looking at run 3 is kinda suprising because after 2 million steps, it did not found a good method of getting towards the goal.

Looking at the results for lower_gamma_config_moveJump_V1_run_1 you can see that it preformed worse than any other runs i did, so we prop don't want to change the gamma from the original starting config.

Next thing we can try is to increase the network complexity by adding more hidden units.

Looking at the results you can see its better than all the starting config runs. It seems like this increase in network complexity worked off quite well.
You can find the results in the higher_hunits_config_moveJump_v1_run_1 file.

The last thing i want to try is to increase the batch and buffer size in hyperparameters.
Looking at the results this did not have any impact on the learning.

From these findings i see that increasing the hidden units in the network settings helped to speed up 
