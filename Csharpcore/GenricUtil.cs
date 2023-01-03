using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csharpcore
{
    class GenricUtil
    {
        static void Main(string[] args)
        {
            /**
            逆变
            ProcessHandlerIn<People>和ProcessHandlerIn<Animal>没有继承关系
            People是Animal的子类
            ProcessHandlerIn<People>的变量指向ProcessHandlerIn<Animal>的实例，形式上类似于：子类的变量指向父类的实例，所以是逆变
            因为类型参数仅作用于参数位置，当子类变量调用方法的时候，传入的参数值类型要求是子类型，而变量实际的参数值类型是父类型，所以调用肯定没问题
            参数值仍然遵循父类的变量指向子类的实例
            */
            ProcessHandlerIn<Animal> phpin = null;
            ProcessHandlerIn<People> phs = phpin;

            /**
            协变
            ProcessHandlerOut<People>ProcessHandlerOut<Student>没有继承关系
            People是Student的父类
            ProcessHandlerOut<People>ProcessHandlerOut<Student>的实例，形式上类似于：父类的变量指向子类的实例，所以是协变
            因为类型参数仅作用于返回值位置，当父类变量调用方法的时候，返回值类型要求是父类型，而变量实际返回值得类型是子类型，所以调用肯定没问题
            返回值仍然遵循父类的变量指向子类的实例
            */
            ProcessHandlerOut<Student> phpout = null;
            ProcessHandlerOut<People> pha = phpout;

        }
    }
}
