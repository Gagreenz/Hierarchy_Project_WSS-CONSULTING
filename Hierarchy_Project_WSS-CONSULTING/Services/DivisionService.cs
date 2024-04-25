using Hierarchy_Project_WSS_CONSULTING.Models;
using Hierarchy_Project_WSS_CONSULTING.Models.DB;
using Hierarchy_Project_WSS_CONSULTING.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Hierarchy_Project_WSS_CONSULTING.Services
{
    public class DivisionService
    {
        private readonly DivisionContext _context;

        public DivisionService(DivisionContext context)
        {
            _context = context;
        }

        public async Task<List<Division>> GetAllAsync()
        {
            var divisions = await _context.Divisions.OrderBy(d => d.PathFromPatriarch).ToListAsync();
            return divisions;
        }

        public async Task AddAsync(NewDivisionDto newDivision)
        {
            if (newDivision == null) return;

            Division division;

            if (newDivision.ParentId == string.Empty)
            {
                //Если нет id родителя значит это новый корень
                division = new Division(HierarchyId.Parse("/"),newDivision.Name);
            }
            else
            {
                var parent = await _context.Divisions.FindAsync(newDivision.ParentId);
                if (parent == null) return;
                //Ищем крайнего потомка чтобы сохранить порядок следования если null то имеет ввиду что это первый потомок
                var lastChild = await _context.Divisions
                    .Where(d => d.PathFromPatriarch.GetAncestor(1) == parent.PathFromPatriarch)
                    .OrderBy(d => d.PathFromPatriarch)
                    .LastOrDefaultAsync();
                //Высчитываем иерархию для нового элемента
                var newDivisionHierarchyPath = parent.PathFromPatriarch.GetDescendant(lastChild?.PathFromPatriarch);

                division = new Division(newDivisionHierarchyPath, newDivision.Name);
            }

            await _context.Divisions.AddAsync(division);
            await _context.SaveChangesAsync();
        }

        public async Task Move(MoveDivisionDto moveDivision)
        {
            var division = await _context.Divisions.FindAsync(moveDivision.OldParentId);
            var oldParentPath = division.PathFromPatriarch.GetAncestor(1);
            var newParent = await _context.Divisions.FindAsync(moveDivision.NewParentId);

            // Получаем всех потомков
            var descendants = await GetDescendantsAsync(division.PathFromPatriarch);

            // Перемещаем division и всех его потомков к новому родителю
            foreach (var descendant in descendants)
            {
                descendant.PathFromPatriarch = descendant.PathFromPatriarch
                    .GetReparentedValue(oldParentPath, newParent.PathFromPatriarch);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<Division>> GetDescendantsAsync(HierarchyId hierarchyId)
        {
            // Рекурсивно получаем всех потомков данного деления
            var descendants = await _context.Divisions
                .Where(e => e.PathFromPatriarch.IsDescendantOf(hierarchyId) && e.PathFromPatriarch != hierarchyId)
                .ToListAsync();

            var allDescendants = new List<Division>();
            foreach (var descendant in descendants)
            {
                allDescendants.Add(descendant);
                var childDescendants = await GetDescendantsAsync(descendant.PathFromPatriarch);
                allDescendants.AddRange(childDescendants);
            }

            return allDescendants;
        }


        public async Task DeleteEntityAndChildrenAsync(string id)
        {
            var entity = await _context.Divisions.FindAsync(id);
            if (entity != null)
            {
                // Проходимся по потомкам для удаления
                await DeleteChildrenAsync(entity.PathFromPatriarch);
                _context.Divisions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        private async Task DeleteChildrenAsync(HierarchyId hierarchyId)
        {
            var children = await _context.Divisions
                .Where(e => e.PathFromPatriarch.IsDescendantOf(hierarchyId) && e.PathFromPatriarch != hierarchyId)
                .ToListAsync();

            foreach (var child in children)
            {
                await DeleteChildrenAsync(child.PathFromPatriarch);
                _context.Divisions.Remove(child);
            }
        }

        // Перезаписываем данные в бд на те что в divisions
        public async Task RewriteDivisions(List<Division> divisions)
        {
            // Подразумевается что корневой division единтсвенный и его path = '/'
            // Удаление было сделано учитывая что нет какого то личного хранения divisions и при загрузке они наслаиваются на друг друга
            var root = await _context.Divisions.FirstOrDefaultAsync(d => d.PathFromPatriarch.ToString() == "/");
            if(root != null)
            {
                await DeleteEntityAndChildrenAsync(root.Id);
            }

            await _context.Divisions.AddRangeAsync(divisions);
            await _context.SaveChangesAsync();
        }
    }
}
