# AdvancedTools_AI
First i ran the same config 3 times to check how much randomness there was while using the same config. 
You can see these runs as starting_config_moveJump_v1_run_1..3

The starting config is based on a web article i found :
https://towardsdatascience.com/an-introduction-to-unity-ml-agents-6238452fcf4c

When looking at the data, you can see that there is a lot of variance in the 3 runs. Looking at run 2, you can see fairly early on that it found a method of getting to the goal. Looking at run 1 it seems to also starting to find something but is not as good as run 2. Looking at run 3 is kinda suprising because after 2 million steps, it did not found a good method of getting towards the goal.

After that i tried to improve up on the config. I do this with the information in the article i linked above and with the official documentation:
https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Training-Configuration-File.md

Looking at the results for lower_gamma_config_moveJump_V1_run_1 you can see that it preformed worse than any other runs i did, so we prop don't want to change the gamma from the original starting config.

Next thing we can try is to increase the network complexity by adding more hidden units.

Looking at the results you can see its better than all the starting config runs. It seems like this increase in network complexity worked off quite well.
You can find the results in the higher_hunits_config_moveJump_v1_run_1 file.

The last thing i want to try is to increase the batch and buffer size in hyperparameters.

