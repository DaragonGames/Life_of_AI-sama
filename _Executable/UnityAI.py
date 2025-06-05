# This AI handels a single request
from gpt4all import GPT4All
import sys

model = GPT4All("Meta-Llama-3-8B-Instruct.Q4_0.gguf")

inp = sys.argv[len(sys.argv)-1]
prompt = inp.replace('_',' ')

with model.chat_session():
    print(model.generate(prompt))

